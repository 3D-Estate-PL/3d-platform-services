namespace InteriorDesigners.Api.InteriorsDesigners.Requests;

public class GetProductsRequest
{
    public List<string>? Ids { get; set; }
    public string? Category { get; set; }
}