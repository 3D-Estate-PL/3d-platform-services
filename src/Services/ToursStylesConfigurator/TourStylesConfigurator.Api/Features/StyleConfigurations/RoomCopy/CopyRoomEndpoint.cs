using TourStylesConfigurator.Api.Features.StyleConfigurations.Model;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Styles;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.RoomCopy;

public class Endpoint : Endpoint<CopyRoomRequest, CopyRoomResponse>
{
    private readonly ITourStyleConfigurationRepository _tourStyleConfigurationRepository;


    public Endpoint(ITourStyleConfigurationRepository tourStyleConfigurationRepository)
    {
        _tourStyleConfigurationRepository = tourStyleConfigurationRepository;
    }

    public override void Configure()
    {
        Post($"/{Paths.TourStyleConfigurations}/{{ConfigurationId}}/{Paths.Place}/{Paths.Styles}/{{ConfigurationStyleId}}/{Paths.Rooms}/{{RoomId}}/clone");
        AllowAnonymous();
        Description(b => b
                .Accepts<CopyRoomRequest>("application/json")
                .Produces<CopyRoomResponse>(200, "application/json"),
            true);
        Summary(s =>
        {
            s.Summary = "Edit room";
            s.Description = "Edit room";
            s.ExampleRequest = new CopyRoomRequest
            {
                ConfigurationId = "61e556cf-eb0a-4aa5-8d9d-02937e93e9c9",
                ConfigurationStyleId = "5e1b3427-9a1e-4cb1-8f5e-ef97ffa699ab",
                Place = PlaceType.Interior,
                RoomId = "Id",
            };
            s.Responses[200] = "Empty";
        });
    }

    public override async Task HandleAsync(CopyRoomRequest request, CancellationToken c)
    {
        var configuration = await _tourStyleConfigurationRepository.GetConfigurationAsync(request.ConfigurationId);

        if (configuration.Status != StyleConfigurationStatus.Draft)
        {
            ThrowError("Can not change style. Configuration is marked as readonly.");
        }
        
        var styleConfiguration = configuration.GetPlace(request.Place).Styles.Single(x => x.Id == request.ConfigurationStyleId);

        var room = styleConfiguration.RoomsConfigurations.Single(x => x.Id == request.RoomId);

        
        var newRoom = room.Clone($"{request.CustomName ?? room.CustomName}");
        styleConfiguration.AddRoom(newRoom);

        await _tourStyleConfigurationRepository.UpsertConfigurationAsync(configuration);

        await SendAsync(new CopyRoomResponse
        {
            ConfigurationId = configuration.Id,
            ConfigurationStyleId = styleConfiguration.Id,
            Room =  new RoomResponseDto
            {
                Id = newRoom.Id,
                Style = new RoomStyleBaseDto
                {
                    BaseStyle = newRoom.SelectedStyle.BaseStyle,
                    IsCustom = newRoom.SelectedStyle.IsCustom
                },
                CustomName = newRoom.CustomName,
                RoomType = newRoom.Type
            }
            
        }, cancellation: c);
    }
}