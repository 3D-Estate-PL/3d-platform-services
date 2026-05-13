using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.TourStyleAdd;

public class AddNewStyleToConfigurationRequest
{
    public string ConfigurationId { get; set; }
    
    public PlaceType Place { get; set; }
    public string CustomName { get; set; }
    public TourStyle BaseStyle { get; set; }
    public List<NewRoomsConfigurationDto> RoomsConfigurations { get; set; }
    
}