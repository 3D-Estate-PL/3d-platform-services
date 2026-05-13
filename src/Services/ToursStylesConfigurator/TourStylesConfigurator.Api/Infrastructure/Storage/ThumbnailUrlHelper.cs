namespace TourStylesConfigurator.Api.Infrastructure.Storage;

public static class ThumbnailUrlHelper
{
    public static string GetUrl(string productId, ImageStorageSettings imageStorageSettings)
    {
        //workaround

        if (productId != null && productId.Contains('_'))
        {
            productId = productId.Split("_")[1];
        }

        var host = imageStorageSettings.ImageServiceUrl;
        return $"{host}/images/products/image_{productId}_0";
    }
}