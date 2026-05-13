using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Styles;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.RoomDelete;

public class Endpoint : Endpoint<DeleteRoomRequest, DeleteRoomResponse>
{
    private readonly ITourStyleConfigurationRepository _tourStyleConfigurationRepository;


    public Endpoint(ITourStyleConfigurationRepository tourStyleConfigurationRepository)
    {
        _tourStyleConfigurationRepository = tourStyleConfigurationRepository;
    }

    public override void Configure()
    {
        Delete($"/{Paths.TourStyleConfigurations}/{{ConfigurationId}}/{Paths.Place}/{Paths.Styles}/{{ConfigurationStyleId}}/{Paths.Rooms}/{{RoomId}}");
        AllowAnonymous();
        Description(b => b
                .Accepts<DeleteRoomRequest>("application/json")
                .Produces<DeleteRoomResponse>(200, "application/json"),
            true);
        Summary(s =>
        {
            s.Summary = "Remove room from configuration style";
            s.Description = "Remove room from configuration style";
            s.ExampleRequest = new DeleteRoomRequest
            {
                ConfigurationId = "61e556cf-eb0a-4aa5-8d9d-02937e93e9c9",
                ConfigurationStyleId = "5e1b3427-9a1e-4cb1-8f5e-ef97ffa699ab",
                RoomId = "RoomId"
            };
            s.Responses[200] = "Empty";
        });
    }

    public override async Task HandleAsync(DeleteRoomRequest request, CancellationToken c)
    {
        var configuration = await _tourStyleConfigurationRepository.GetConfigurationAsync(request.ConfigurationId);
        
        if (configuration.Status != StyleConfigurationStatus.Draft)
        {
            ThrowError("Can not change style. Configuration is marked as readonly.");
        }
        
        var styleConfiguration = configuration.GetPlace(request.Place).Styles.Single(x => x.Id == request.ConfigurationStyleId);

        var room = styleConfiguration.RoomsConfigurations.Single(x => x.Id == request.RoomId);

        if (room.IsRequired)
        {
            ThrowError("Nie można usunąnac wymaganego pomieszenia.");
        }
        
        styleConfiguration.RemoveRoom(room);
        
        await _tourStyleConfigurationRepository.UpsertConfigurationAsync(configuration);

        await SendAsync(new DeleteRoomResponse(), cancellation: c);
    }
}