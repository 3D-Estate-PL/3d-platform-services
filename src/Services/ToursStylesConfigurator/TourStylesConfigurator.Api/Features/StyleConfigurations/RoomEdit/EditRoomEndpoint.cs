using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Styles;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.RoomEdit;

public class Endpoint : Endpoint<EditRoomRequest, EditRoomResponse>
{
    private readonly ITourStyleConfigurationRepository _tourStyleConfigurationRepository;


    public Endpoint(ITourStyleConfigurationRepository tourStyleConfigurationRepository)
    {
        _tourStyleConfigurationRepository = tourStyleConfigurationRepository;
    }

    public override void Configure()
    {
        Put(
            $"/{Paths.TourStyleConfigurations}/{{ConfigurationId}}/{Paths.Place}/{Paths.Styles}/{{ConfigurationStyleId}}/{Paths.Rooms}/{{RoomId}}");
        AllowAnonymous();
        Description(b => b
                .Accepts<EditRoomRequest>("application/json")
                .Produces<EditRoomResponse>(200, "application/json"),
            true);
        Summary(s =>
        {
            s.Summary = "Edit room";
            s.Description = "Edit room";
            s.ExampleRequest = new EditRoomRequest
            {
                ConfigurationId = "61e556cf-eb0a-4aa5-8d9d-02937e93e9c9",
                ConfigurationStyleId = "5e1b3427-9a1e-4cb1-8f5e-ef97ffa699ab",
                RoomId = "Id",
                Place = PlaceType.Interior,
                Room = new EditRoomRequestDto
                {
                    CustomName = "Test1",
                    RoomType = "kidsRoom",
                    BaseStyle = new TourStyle
                    {
                        Group = "Basic",
                        Kind = "Berlin"
                    }
                }
            };
            s.Responses[200] = "Empty";
        });
    }

    public override async Task HandleAsync(EditRoomRequest request, CancellationToken c)
    {
        var configuration = await _tourStyleConfigurationRepository.GetConfigurationAsync(request.ConfigurationId);

        if (configuration.Status != StyleConfigurationStatus.Draft)
        {
            ThrowError("Can not change style. Configuration is marked as readonly.");
        }

        var styleConfiguration = configuration.GetPlace(request.Place).Styles
            .Single(x => x.Id == request.ConfigurationStyleId);

        var room = styleConfiguration.RoomsConfigurations.Single(x => x.Id == request.RoomId);
        room.CustomName = request.Room.CustomName;
        room.Type = request.Room.RoomType;
        room.AllowOverrideBaseConfiguration = request.AllowOverrideBaseConfiguration;
        room.SelectedStyle = RoomStyle.New(request.Room.BaseStyle);

        await _tourStyleConfigurationRepository.UpsertConfigurationAsync(configuration);

        await SendAsync(new EditRoomResponse
        {
            ConfigurationId = configuration.Id,
            ConfigurationStyleId = styleConfiguration.Id,
            Room = new EditRoomResponseDto
            {
                Id = room.Id,
                CustomName = room.CustomName,
                RoomType = room.Type,
                BaseStyle = room.SelectedStyle.BaseStyle
            }
        }, cancellation: c);
    }
}