namespace TourStylesConfigurator.Api.Features.InteriorsDesigners.Products.GetProductDetails;

public record struct GetProductDetailResponse
{
   public InteriorDesignerProductDto Product { get; set; }
}

public class Request
{
    public string Id { get; set; }
    public string? InteriorDesigner { get; set; }
};