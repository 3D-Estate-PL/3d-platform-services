using BuildingBlocks.Abstractions.CQRS.CQRS;
using CoreProducts.Application.Products.Dtos;

namespace CoreProducts.Application.Products.Queries.GetProducts;

public class GetProductsResponse
{
    public GetProductsResponse(List<CoreProductDto> products)
    {
        Products = products;
    }

    public List<CoreProductDto> Products { get; set; }
}

public class GetProductsQuery : IQuery<GetProductsResponse>
{
    public List<string>? Ids { get; set; }
}