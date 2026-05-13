namespace TourStylesConfigurator.Api.Features.StyleConfigurations.GetConfigurationStylesForPlace;

public class GetConfigurationStylesResponse
{
    public GetConfigurationStylesResponse()
    {
        Styles = new List<ConfigurationStyleItemDto>();
    }

    public string ConfigurationId { get; set; }
    public List<ConfigurationStyleItemDto> Styles { get; set; }
}