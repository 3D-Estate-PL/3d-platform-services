using TourStylesConfigurator.Api.Features.StyleConfigurations.ConfigurationAdd;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.ConfigurationEdit;

public class EditTourStylesConfigurationRequest
{
    public string ConfigurationId { get; set; }
    public OwnerDto Owner { get; set; }

    public string InvestmentName { get; set; }
}

public class EditTourStylesConfigurationResponse
{
    public string ConfigurationId { get; set; }
    public OwnerDto Owner { get; set; }
    public string InvestmentName { get; set; }
}