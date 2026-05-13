using BuildingBlocks.Abstractions.CQRS.CQRS;
using InteriorDesigners.Application.Products.Dtos;

namespace InteriorDesigners.Application.Products.GetProducts;

public class GetProductDetailsQuery : IQuery<GetProductDetailResponse>
{
    public string Id { get; set; }
}


public record struct GetProductDetailResponse
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