namespace TourStylesConfigurator.Api.Features.StyleConfigurations.ImportConfiguration;

public class ImportStylesConfigurationRequest
{
    public string Email { get; set; }
    public string Code { get; set; }
    
}


public class ImportStylesConfigurationResponse
{
    public CreateStyleConfigurationDto StyleConfiguration { get; set; }
}