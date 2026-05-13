namespace CoreProducts.Domain.Products;

public class ProductCategory
{

    public required string Type { get; init; }
    public string? DisplayName { get; set; }
}