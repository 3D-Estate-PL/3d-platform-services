namespace TourStylesConfigurator.Api.Features.StyleConfigurations.Services.Commons;

public static class ListExtensions
{
    public static List<T> Clone<T>(this List<T> listToClone) where T : ICloneable
    {
        return listToClone.Select(item => (T) item.Clone()).ToList();
    }
}