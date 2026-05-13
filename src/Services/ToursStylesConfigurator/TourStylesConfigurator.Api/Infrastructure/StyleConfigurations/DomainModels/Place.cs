using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Styles;

namespace TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;

public class Place
{
    public List<StyleConfigurationItem> Styles { get; set; }
    
    public int StyleCount { get; set; }
    public List<StyleConfigurationItem> Clone()
    {
        var number = 1;
        var result = Styles.ConvertAll(s => s.Clone($"{number++}"));
        return result;
    }

    public void SetStyles(List<StyleConfigurationItem> styles)
    {
        Styles = styles;
    }

    public void AddStyle(StyleConfigurationItem styleConfiguration)
    {
        StyleCount++;
        styleConfiguration.Code = $"{StyleCount}";
        Styles.Add(styleConfiguration);
    }
    
    public void RemoveStyle(StyleConfigurationItem styleConfiguration)
    {
        StyleCount--;
        Styles.Remove(styleConfiguration);
    }
}