namespace TourStylesConfigurator.Api.Features.InteriorsDesigners.Products.GetProductDetailsV2;

public record struct GetProductDetailResponse2
{
    public string Id { get; set; }

    public  string Name { get; set; }
    
    public string Type { get; set; }
    
    public IList<string> Categories { get; set; }
    
    public IList<string> SubCategories { get; set; }

    public string Unit { get; set; }
    
    public IList<string> Styles { get; set; }
    
    public List<InteriorDesignerProductDto> Offers { get; set; }
}

public class Request
{
    public string Id { get; set; }
};