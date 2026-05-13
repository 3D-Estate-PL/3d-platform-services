namespace TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;

public class TourStyle
{
    public string Group { get; set; }
    public string Kind { get; set; }
    
    
    public override string ToString()
    {
        return $"Basic.{Kind}";
    }
}