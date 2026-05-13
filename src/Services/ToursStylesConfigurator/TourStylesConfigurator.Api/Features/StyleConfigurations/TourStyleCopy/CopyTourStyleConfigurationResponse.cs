using TourStylesConfigurator.Api.Features.StyleConfigurations.Model;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.TourStyleCopy;

public class CopyTourStyleConfigurationResponse
{
    public string Id { get; set; }
    public string CustomName { get; set; }
    public List<RoomConfigurationDto> RoomsConfigurations { get; set; }
}