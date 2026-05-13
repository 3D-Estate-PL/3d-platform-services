namespace TourStylesConfigurator.Api.Features.StyleConfigurations.ConfigurationAdd;

public class CreateNewConfigurationRequest
{
    public OwnerDto ConfigurationOwner { get; set; }
    public string InvestmentName { get; set; }
}

public class OwnerDto
{
    public string Name { get; set; }
    public string Email { get; set; }
}

public class CreateNewStyleConfigurationResponse
{
    public StyleConfigurationDto StyleConfiguration { get; set; }
}