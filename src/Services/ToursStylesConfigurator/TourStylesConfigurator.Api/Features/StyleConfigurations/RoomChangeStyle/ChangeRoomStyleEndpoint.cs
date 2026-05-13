using TourStylesConfigurator.Api.Features.StyleConfigurations.Model;
using TourStylesConfigurator.Api.Features.StyleConfigurations.RoomGetAvailableStyles;
using TourStylesConfigurator.Api.Features.StyleConfigurations.Services;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Styles;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.RoomChangeStyle;

public class ChangeRoomStyleEndpoint : Endpoint<ChangeRoomStyleRequest, ChangeRoomStyleResponse>
{
    private readonly ITourStyleConfigurationRepository _tourStyleConfigurationRepository;
    private IStyleDefaultProductsProvider _styleDefaultProductsProvider;
    private readonly IGetTourStylesQuery _getTourStylesQuery;


    public ChangeRoomStyleEndpoint(ITourStyleConfigurationRepository tourStyleConfigurationRepository, IStyleDefaultProductsProvider styleDefaultProductsProvider, IGetTourStylesQuery getTourStylesQuery)
    {
        _tourStyleConfigurationRepository = tourStyleConfigurationRepository;
        _styleDefaultProductsProvider = styleDefaultProductsProvider;
        _getTourStylesQuery = getTourStylesQuery;
    }

    public override void Configure()
    {
        Put($"/{Paths.TourStyleConfigurations}/{{ConfigurationId}}/{Paths.Place}/{Paths.Styles}/{{ConfigurationStyleId}}/{Paths.Rooms}/{{RoomId}}/style");
        AllowAnonymous();
        Description(b => b
                .Accepts<ChangeRoomStyleRequest>("application/json")
                .Produces<ChangeRoomStyleResponse>(200, "application/json"),
            true);
        Summary(s =>
        {
            s.Summary = "Change room style";
            s.Description = "Edit room";
            s.ExampleRequest = new ChangeRoomStyleRequest
            {
                ConfigurationId = "61e556cf-eb0a-4aa5-8d9d-02937e93e9c9",
                ConfigurationStyleId = "5e1b3427-9a1e-4cb1-8f5e-ef97ffa699ab",
                RoomId = "Id",
                Place = PlaceType.Interior,
                Style = new RoomStyleBaseDto
                {
                    BaseStyle =  new TourStyle
                    {
                        Group = "Basic",
                        Kind = "Berlin"
                    }
                }
            };
            s.Responses[200] = "Empty";
        });
    }

    public override async Task HandleAsync(ChangeRoomStyleRequest request, CancellationToken c)
    {
        if (request.Style == null || request.Style.BaseStyle == null && request.Style.IsCustom == false)
        {
            ThrowError("Style is required.");
        }
        
        var configuration = await _tourStyleConfigurationRepository.GetConfigurationAsync(request.ConfigurationId);

        if (configuration.Status != StyleConfigurationStatus.Draft)
        {
            ThrowError("Can not change style. Configuration is marked as readonly.");
        }
        
        var styleConfiguration = configuration.GetPlace(request.Place).Styles.Single(x => x.Id == request.ConfigurationStyleId);

        var room = styleConfiguration.RoomsConfigurations.Single(x => x.Id == request.RoomId);
        var availableStylesForRoom = await _getTourStylesQuery.GetTourStyles(room.Type);

        if (request.Style.IsCustom)
        {
            room.SetCustomStyle();
        }
        else
        {
            var defaultProducts = await _styleDefaultProductsProvider.GetDefaultProductsForRoom(room.Type, request.Style.BaseStyle.ToString());
            room.SetBaseStyle(request.Style.BaseStyle, defaultProducts.Map(room.Type));
        }

        await _tourStyleConfigurationRepository.UpsertConfigurationAsync(configuration);
        var roomStyle = availableStylesForRoom.SingleOrDefault(x => x.TourStyle.Kind == room.SelectedStyle.BaseStyle.Kind);
        
        await SendAsync(new ChangeRoomStyleResponse
        {
            ConfigurationId = configuration.Id,
            ConfigurationStyleId = styleConfiguration.Id,
            Room =  new EditRoomResponseDto
                {
                    Id = room.Id,
                    CustomName = room.CustomName,
                    RoomType = room.Type,
                    Style = new RoomStyleDto(room.SelectedStyle.BaseStyle,room.SelectedStyle.IsCustom, roomStyle?.DisplayName, roomStyle?.Description, roomStyle?.ThumbnailUrl),
                    Products = room.GetProducts()
                }
            
        }, cancellation: c);
    }
}