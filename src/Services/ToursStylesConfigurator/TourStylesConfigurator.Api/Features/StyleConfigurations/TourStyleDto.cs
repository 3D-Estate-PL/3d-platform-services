using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations;

public class TourStyleDto
{
    public TourStyle TourStyle { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    
    public string RoundThumbnailUrl { get; set; }
    public string ThumbnailUrl { get; set; }
    
    public string ImageUrl => ThumbnailUrl;

    public List<string> Images { get; set; } = new List<string>();

    public List<StyleContext> Contexts { get; set; } = new List<StyleContext>();
}

public enum StyleContext
{
    Configurator,
    Platform,
    WithoutContext
}