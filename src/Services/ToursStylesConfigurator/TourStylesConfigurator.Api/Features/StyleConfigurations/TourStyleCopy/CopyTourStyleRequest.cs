using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.TourStyleCopy;

public class CopyTourStyleRequest
{
    public string ConfigurationId { get; set; }
    public PlaceType Place { get; set; }
    
    public string ConfigurationStyleId { get; set; }
    public string CustomName { get; set; }
    
}