using TourStylesConfigurator.Api.Infrastructure.Storage;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;

namespace TourStylesConfigurator.Api.Features;

public static class StylesHelper
{
    public static string GetGenericImageUrl(TourStyle style, string roomTypeName, ImageStorageSettings imageStorageSettings)
    {
       
        var url = $"{ imageStorageSettings.ImageServiceUrl}/images/styles-images/basic.{style.Kind.ToLower()}.{roomTypeName.ToLower()}";
        return url;
    }
    
    public static string GetStyleThumbnailByConventionUrl(TourStyle style, string roomTypeName,ImageStorageSettings imageStorageSettings)
    {
        var url =$"{imageStorageSettings.ImageServiceUrl}/images/styles-images/basic.{style.Kind.ToLower()}.{roomTypeName.ToLower()}";
        return url;
    }
}