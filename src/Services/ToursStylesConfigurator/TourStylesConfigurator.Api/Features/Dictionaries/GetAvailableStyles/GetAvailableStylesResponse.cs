using TourStylesConfigurator.Api.Features.StyleConfigurations;

namespace TourStylesConfigurator.Api.Features.Dictionaries.GetAvailableStyles;

public class GetAvailableStylesResponse
{
    public List<TourStyleDto> TourStyles { get; set; }
}