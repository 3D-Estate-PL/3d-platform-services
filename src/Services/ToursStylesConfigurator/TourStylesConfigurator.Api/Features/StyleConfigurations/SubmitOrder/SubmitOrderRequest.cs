namespace TourStylesConfigurator.Api.Features.StyleConfigurations.SubmitOrder;

public class SubmitOrderRequest
{
    public string ConfigurationId { get; set; }

    public string[] SelectedStylesIds { get; set; }
}

