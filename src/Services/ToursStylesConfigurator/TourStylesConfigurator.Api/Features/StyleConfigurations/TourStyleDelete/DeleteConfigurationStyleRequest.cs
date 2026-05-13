using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.TourStyleDelete;

public class DeleteConfigurationStyleRequest
{
    public string ConfigurationId { get; set; }
    public PlaceType Place { get; set; }
    public string ConfigurationStyleId { get; set; }
}