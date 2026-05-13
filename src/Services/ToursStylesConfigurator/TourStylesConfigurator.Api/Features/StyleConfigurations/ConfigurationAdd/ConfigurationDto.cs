using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Styles;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.ConfigurationAdd;

public class StyleConfigurationDto
{
    public string Id { get; set; }
    public string Code { get; set; }
    public StyleConfigurationStatus Status { get; set; }
    public string InvestmentName { get; set; }
    public Owner Owner { get; set; }
    public bool IsEditable { get; set; }
}

public class Owner
{
    public string Id { get; set; }
    public string Email { get; set; }
    
    public string Name { get; set; }
}
