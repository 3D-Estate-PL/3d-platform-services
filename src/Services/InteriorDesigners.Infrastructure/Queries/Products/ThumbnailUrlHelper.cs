namespace InteriorDesigners.Infrastructure.Queries.Products;

public static class ThumbnailUrlHelper
{
    public static string GetUrl(string productId, ImageStorageSettings imageStorageSettings)
    {
        //workaround

        if (productId != null && productId.Contains('_'))
        {
            productId = productId.Split("_")[1];
        }
        
        return $"{imageStorageSettings.ImageServiceUrl}/images/products/image_{productId}_0";
    }
}