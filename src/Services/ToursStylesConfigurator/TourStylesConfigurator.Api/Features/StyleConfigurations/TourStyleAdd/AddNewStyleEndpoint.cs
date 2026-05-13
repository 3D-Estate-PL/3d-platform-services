using TourStylesConfigurator.Api.Features.StyleConfigurations.Model;
using TourStylesConfigurator.Api.Features.StyleConfigurations.Services;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Styles;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.TourStyleAdd;

public class Endpoint : Endpoint<AddNewStyleToConfigurationRequest, AddNewStyleToConfigurationResponse>
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
        Post($"/{Paths.TourStyleConfigurations}/{{ConfigurationId}}/{Paths.Place}/{Paths.Styles}");
        AllowAnonymous();
        Description(b => b
                .Accepts<AddNewStyleToConfigurationRequest>("application/json")
                .Produces<AddNewStyleToConfigurationResponse>(200, "application/json"),
            true);
        Summary(s =>
        {
            s.Summary = "Add new style to tour configuration";
            s.Description = "Add new style to tour configuration";
            s.ExampleRequest = new AddNewStyleToConfigurationRequest
            {
                ConfigurationId = "61e556cf-eb0a-4aa5-8d9d-02937e93e9c9",
                CustomName = $"Nowoczesne mieszkanie dla Par [{DateTime.Now.Ticks}]",
                BaseStyle = new TourStyle
                {
                    Group = "Basic",
                    Kind = "Mediolan"
                },
                RoomsConfigurations = new List<NewRoomsConfigurationDto>
                {
                    new NewRoomsConfigurationDto
                    {
                        RoomType = "common"
                    },
                    new NewRoomsConfigurationDto
                    {
                        RoomType = "livingRoom"
                    },
                    new NewRoomsConfigurationDto
                    {
                        RoomType = "bathroom"
                    }
                }
            };
            s.Responses[200] = "Empty";
        });
    }

    public override async Task HandleAsync(AddNewStyleToConfigurationRequest request, CancellationToken c)
    {
        var userLangs = HttpContext.Request.Headers["accept-language"].ToString();
        var language = userLangs.Split(',').FirstOrDefault();
        
        var configuration = await _tourStyleConfigurationRepository.GetConfigurationAsync(request.ConfigurationId);
        
        if (configuration.Status != StyleConfigurationStatus.Draft)
        {
            ThrowError("Can not change style. Configuration is marked as readonly.");
        }
        
        var roomTypes = await _getRoomTypesQuery.GetTourConfigurationRoomTypes(language);
        var requiredRoomsTypes = roomTypes.Where(x=>x.IsRequired).ToList();

        var defaultProducts = await _styleDefaultProductsProvider.
            GetDefaultProducts(request.BaseStyle.ToString());

        var interiorStyleConfiguration = StyleConfigurationItem.Interior(requiredRoomsTypes, 
            defaultProducts,
            request.BaseStyle, request.CustomName);


        foreach (var roomsConfigurations in request.RoomsConfigurations.
                     Where(x=>roomTypes.Any(y=>string.Equals(y.RoomType, x.RoomType, StringComparison.OrdinalIgnoreCase) &&
                                               y.IsRequired == false)))
        {
            var availableRoomTypeDto = roomTypes.SingleOrDefault(x=>
                string.Equals(x.RoomType,roomsConfigurations.RoomType,StringComparison.OrdinalIgnoreCase));

            if (availableRoomTypeDto == null)
            {
                ThrowError($"RoomType {roomsConfigurations.RoomType} not found.");
            }

            interiorStyleConfiguration.AddRoom(RoomItem.New(availableRoomTypeDto.DisplayName, roomsConfigurations.RoomType, RoomStyle.New(request.BaseStyle), 
                defaultProducts.Map(roomsConfigurations.RoomType),isRequired:availableRoomTypeDto.IsRequired));
        }

        if (roomTypes.Count(x => x.IsRequired) != interiorStyleConfiguration.RoomsConfigurations.Count(y => y.IsRequired))
        {
            ThrowError($"Required rooms did not add.");
        }
        
        if (configuration.GetPlace(request.Place).Styles == null)
        {
            configuration.GetPlace(request.Place).Styles = new List<StyleConfigurationItem>();
        }

        configuration.GetPlace(request.Place).AddStyle(interiorStyleConfiguration);

        await _tourStyleConfigurationRepository.UpsertConfigurationAsync(configuration);


        var response = new AddNewStyleToConfigurationResponse
        {
            Id  = interiorStyleConfiguration.Id,
            CustomName = interiorStyleConfiguration.CustomName,
            Code = interiorStyleConfiguration.Code,
            RoomsConfigurations = interiorStyleConfiguration.RoomsConfigurations.Select(x=> new RoomConfigurationDto
            {
                Id = x.Id,
                CustomName = x.CustomName,
                RoomType = x.Type,
                IsRequired = x.IsRequired,
                Style = new RoomStyleBaseDto
                {
                    BaseStyle = x.SelectedStyle.BaseStyle,
                    IsCustom = x.SelectedStyle.IsCustom
                }
                
            }).ToList()
        };
        
         
        await SendAsync(response, cancellation: c);
    }
}