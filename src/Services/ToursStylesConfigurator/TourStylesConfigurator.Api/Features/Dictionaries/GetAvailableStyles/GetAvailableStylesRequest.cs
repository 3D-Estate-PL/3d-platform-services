using TourStylesConfigurator.Api.Features.StyleConfigurations;

namespace TourStylesConfigurator.Api.Features.Dictionaries.GetAvailableStyles;

public class GetAvailableStylesRequest
{
    public bool? ClearCache { get; set; }

    /// <summary>
    /// Default selected context is Configurator
    /// </summary>
    public List<StyleContext>? Contexts { get; set; } = new List<StyleContext> {StyleContext.Configurator};
}

