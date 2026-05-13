using TourStylesConfigurator.Api.Features.InteriorsDesigners.Products.GetProductDetails;

namespace TourStylesConfigurator.Api.Features.InteriorsDesigners.Products.GetProducts;

public class GetProductsRequest
{
    public List<string>? Ids { get; set; }
    public string? Category { get; set; }
    
    public string? InteriorDesigner { get; set; }
}

public record struct GetProductsResponse(List<CategoryProductsDto> Products, CategoryDto Category);