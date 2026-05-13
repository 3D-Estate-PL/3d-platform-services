namespace CoreProducts.Application.Products.IntegrationEvents.Events;

public class ProductCategoryDto
{
    public required string Type { get; init; }
    public string? DisplayName { get; set; }
}