using TourStylesConfigurator.Api.Features.StyleConfigurations.Services;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Styles;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.RoomAdd;

public class Endpoint : Endpoint<AddNewRoomRequest, AddNewRoomResponse>
{
    private readonly ITourStyleConfigurationRepository _tourStyleConfigurationRepository;
    private readonly IStyleDefaultProductsProvider _styleDefaultProductsProvider;
    private readonly IGetRoomTypesQuery _getRoomTypesQuery;


    public Endpoint(ITourStyleConfigurationRepository tourStyleConfigurationRepository, IStyleDefaultProductsProvider styleDefaultProductsProvider, IGetRoomTypesQuery getRoomTypesQuery)
    {
        _tourStyleConfigurationRepository = tourStyleConfigurationRepository;
        _styleDefaultProductsProvider = styleDefaultProductsProvider;
        _getRoomTypesQuery = getRoomTypesQuery;
    }

    public override void Configure()
    {
        Post($"/{Paths.TourStyleConfigurations}/{{ConfigurationId}}/{Paths.Place}/{Paths.Styles}/{{ConfigurationStyleId}}/{Paths.Rooms}");
        AllowAnonymous();
        Description(b => b
                .Accepts<AddNewRoomRequest>("application/json")
                .Produces<AddNewRoomResponse>(200, "application/json"),
            true);
        Summary(s =>
        {
            s.Summary = "Add new room"; 
            s.Description = "Add new room";
            s.ExampleRequest = new AddNewRoomRequest
            {
                ConfigurationId = "61e556cf-eb0a-4aa5-8d9d-02937e93e9c9",
                ConfigurationStyleId = "5e1b3427-9a1e-4cb1-8f5e-ef97ffa699ab",
                Place = PlaceType.Interior,
                Rooms = new List<NewRoomRequestDto>
                {
                    new NewRoomRequestDto 
                    {
                        CustomName = "Pokój dziecięcy 2",
                        RoomType = "kidsRoom"
                    }
                }
            };
            s.Responses[200] = "Empty";
        });
    }

    public override async Task HandleAsync(AddNewRoomRequest request, CancellationToken c)
    {
        var userLangs = HttpContext.Request.Headers["accept-language"].ToString();
        var language = userLangs.Split(',').FirstOrDefault();
        
        var configuration = await _tourStyleConfigurationRepository.GetConfigurationAsync(request.ConfigurationId);
        
        if (configuration.Status != StyleConfigurationStatus.Draft)
        {
            ThrowError("Can not change style. Configuration is marked as readonly.");
        }
        
        
        var styleConfiguration = configuration.GetPlace(request.Place).Styles.Single(x => x.Id == request.ConfigurationStyleId);

        var defaultProducts = await _styleDefaultProductsProvider.GetDefaultProducts(styleConfiguration.BaseStyle.ToString());

        var roomTypes = await _getRoomTypesQuery.GetTourConfigurationRoomTypes(language);

        
        foreach (var newRoom in request.Rooms)
        {
            var defaultProductsForRoomType = defaultProducts.Map(newRoom.RoomType);

            styleConfiguration.RoomTypeIndex.TryGetValue(newRoom.RoomType, out var roomIndex);
            var displayName = roomTypes.SingleOrDefault(x => x.RoomType == newRoom.RoomType)?.DisplayName;
            var roomItem = RoomItem.New($"{displayName} {++roomIndex}"
                , newRoom.RoomType, RoomStyle.New(styleConfiguration.BaseStyle), defaultProductsForRoomType);
          
            styleConfiguration.AddRoom(roomItem);
        }


        await _tourStyleConfigurationRepository.UpsertConfigurationAsync(configuration);

        await SendAsync(new AddNewRoomResponse
        {
            ConfigurationId = configuration.Id,
            ConfigurationStyleId = styleConfiguration.Id,
            Rooms = styleConfiguration.RoomsConfigurations.Select(x=>  new NewRoomResponseDto
            {
                Id = x.Id,
                RoomType = x.Type,
                CustomName = x.CustomName,
                IsRequired = x.IsRequired
                
            }).ToList()
        }, cancellation: c);
    }
}