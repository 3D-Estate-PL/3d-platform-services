using TourStylesConfigurator.Api.Features.StyleConfigurations.ConfigurationAdd;
using TourStylesConfigurator.Api.Features.StyleConfigurations.GetConfigurationStylesForPlace;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.ConfigurationCopy;

public class CloneTourStylesConfigurationRequest
{
    public string ConfigurationId { get; set; }
    
    public OwnerDto ConfigurationOwner { get; set; }
    
    public string InvestmentName { get; set; }
}

public class CloneTourStylesConfigurationResponse
{
    public StyleConfigurationDto StyleConfiguration { get; set; }

    public List<ConfigurationStyleItemDto> Styles { get; set; } = new List<ConfigurationStyleItemDto>();
}