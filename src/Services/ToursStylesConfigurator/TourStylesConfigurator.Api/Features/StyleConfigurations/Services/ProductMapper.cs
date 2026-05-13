using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Styles;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.Services;

public static class ProductMapper
{
    public static List<string> CommonCategoryFilters =  new List<string> {"Wall", "Floor", "Curtains", "Baseboard"}.Select(x => x.ToLower()).ToList();

    public static List<ProductItem> Map(this IEnumerable<DefaultProductsDto> defaultStyleProductsDto, string roomType)
    {
        var defaultProductsForRoomType = defaultStyleProductsDto.Single(x =>
                string.Equals(x.RoomType, roomType, StringComparison.InvariantCultureIgnoreCase))
            .DefaultProducts.ToList();

        return Map(defaultProductsForRoomType, roomType);
    }
    
    
    public static List<ProductItem> Map(this IEnumerable<ProductCategoryDto> defaultStyleProductsDto, string roomType)
    {
        var defaultProductsForRoomType = defaultStyleProductsDto
            .Select(x => new ProductItem
            {
                CategoryName = x.CategoryName,
                ProductId = x.ProductId,
                ProductSource = ProductSource.RoomStyle
            }).ToList();

        UpdateProductSourceInformation(defaultProductsForRoomType, roomType);

        return defaultProductsForRoomType;
    }

    private static void UpdateProductSourceInformation(List<ProductItem> roomProducts, string roomType)
    {
        if (string.Equals(roomType.ToLower(), "bathroom")) return;
        
        foreach (var roomProduct in roomProducts)
        {
            if (CommonCategoryFilters.Contains(roomProduct.CategoryName.ToLower()))
                roomProduct.ProductSource = ProductSource.CommonStyle;
        }
    }
}