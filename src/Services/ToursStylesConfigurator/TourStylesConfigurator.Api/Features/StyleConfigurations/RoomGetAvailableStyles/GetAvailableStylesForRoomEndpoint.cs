using TourStylesConfigurator.Api.Features.StyleConfigurations.GetConfigurationStyles;
using TourStylesConfigurator.Api.Features.StyleConfigurations.Services;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.RoomGetAvailableStyles;

public class Endpoint : Endpoint<GetAvailableStylesForRoomRequest, GetAvailableStylesForRoomResponse>
{
    private readonly ITourStyleConfigurationRepository _tourStyleConfigurationRepository;
    private readonly IGetTourStylesQuery _getTourStylesQuery;


    public Endpoint(ITourStyleConfigurationRepository tourStyleConfigurationRepository, IGetTourStylesQuery getTourStylesQuery)
    {
        _tourStyleConfigurationRepository = tourStyleConfigurationRepository;
        _getTourStylesQuery = getTourStylesQuery;
    }

    public override void Configure()
    {
        Get($"/{Paths.TourStyleConfigurations}/{{ConfigurationId}}/{Paths.Place}/{Paths.Styles}/{{ConfigurationStyleId}}/{Paths.Rooms}/{{RoomId}}/availablestyles");
        AllowAnonymous();
        Description(b => b
                .Produces<GetAvailableStylesForRoomResponse>(200, "application/json"),
            clearDefaults: true);
        Summary(s => {
            s.ExampleRequest = new GetAvailableStylesForRoomRequest()
            {
                ConfigurationId = "61e556cf-eb0a-4aa5-8d9d-02937e93e9c9",
                ConfigurationStyleId = "5e1b3427-9a1e-4cb1-8f5e-ef97ffa699ab",
                RoomId = "RoomId"
            };
            s.Summary = "Get available styles for room";
            s.Description =  "Get available styles for room";
            s.Responses[200] = "Empty";
        });
    }

    public override async Task HandleAsync(GetAvailableStylesForRoomRequest request, CancellationToken c)
    {
        var configuration = await _tourStyleConfigurationRepository.GetConfigurationAsync(request.ConfigurationId);


        var configurationStyle =
            configuration.GetPlace(request.Place).Styles.Single(x => x.Id == request.ConfigurationStyleId);

        var room = configurationStyle.RoomsConfigurations.Single(x => x.Id == request.RoomId);

        var tourStyles = await _getTourStylesQuery.GetTourStyles(room.Type.ToPascalCase());
        var result = new List<RoomStyleDto>();
        
        var availableStyles = tourStyles.Where(x=>x.Contexts.Contains(StyleContext.Configurator)).
            Select(styleDefinition =>
            new RoomStyleDto(styleDefinition.TourStyle,false, styleDefinition?.DisplayName, styleDefinition?.Description, styleDefinition?.ThumbnailUrl)
        ).ToList();
        
        if (room.CustomStyle != null)
        {
            result.Add( new RoomStyleDto(room.CustomStyle,
                true,
                tourStyles.SingleOrDefault(x=>x.TourStyle.Kind == room.CustomStyle.Kind)?.DisplayName, 
                tourStyles.SingleOrDefault(x=>x.TourStyle.Kind == room.CustomStyle.Kind)?.Description,
                tourStyles.SingleOrDefault(x=>x.TourStyle.Kind == room.CustomStyle.Kind)?.ThumbnailUrl));
        }
        
        result.AddRange(availableStyles);
 
        await SendAsync(new GetAvailableStylesForRoomResponse()
        {
            RoomId = room.Id,
            AvailableStyles = result
        }, cancellation: c);
    }


}