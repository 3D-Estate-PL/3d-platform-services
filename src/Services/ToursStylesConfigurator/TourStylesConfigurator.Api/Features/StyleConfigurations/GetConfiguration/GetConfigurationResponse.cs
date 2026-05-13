namespace TourStylesConfigurator.Api.Features.StyleConfigurations.GetConfiguration;

public class GetConfigurationResponse
{
    public GetConfigurationResponse()
    {
        Styles = new List<TourConfigurationStyle>();
    }

    public List<TourConfigurationStyle> Styles { get; set; }
}

public class TourConfigurationStyle
{
    public string Code { get; set; }
    public string Name { get; set; }
}