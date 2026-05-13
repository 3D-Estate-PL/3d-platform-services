using BuildingBlocks.Abstractions.CQRS.CQRS;
using InteriorDesigners.Application.Products.Dtos;

namespace InteriorDesigners.Application.Products.GetProducts;

public class GetInteriorDesignerProductDetailsQueryResponse
{
    public InteriorDesignerProductDto Product { get; init; }
}


public class GetInteriorDesignerProductDetailsQuery : IQuery<GetInteriorDesignerProductDetailsQueryResponse>
{
    public string Id { get; set; }
    public string InteriorDesigner { get; set; }
    
    public bool FilterByCoreProductId { get; set; }
}