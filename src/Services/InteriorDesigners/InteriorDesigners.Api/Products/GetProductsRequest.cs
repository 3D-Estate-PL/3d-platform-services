namespace InteriorDesigners.Api.Products;

public class GetProductsRequest
{
    public List<string>? Ids { get; set; }
    public string? Category { get; set; }
    public string? InteriorDesigner { get; set; }
}