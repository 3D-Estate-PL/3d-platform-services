using TourStylesConfigurator.Api.Features.StyleConfigurations.Model;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.TourStyleAdd;

public class AddNewStyleToConfigurationResponse
{
    public string Id { get; set; }
    public string CustomName { get; set; }
    
    public string Code { get; set; }
    public List<RoomConfigurationDto> RoomsConfigurations { get; set; }
}