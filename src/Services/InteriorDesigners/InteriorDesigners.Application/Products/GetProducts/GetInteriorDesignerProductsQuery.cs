using BuildingBlocks.Abstractions.CQRS.CQRS;
using InteriorDesigners.Application.Products.Dtos;

namespace InteriorDesigners.Application.Products.GetProducts;


    public class GetProductsQueryResponse
    {
        public GetProductsQueryResponse(List<CategoryProductsDto> products)
        {
            Products = products;
        }

        public List<CategoryProductsDto> Products { get; set; }
    }


    public class GetInteriorDesignerProductsQuery : IQuery<GetProductsQueryResponse>
    {
        public List<string>? Ids { get; set; }
        public string? Category { get; set; }
        public string InteriorDesigner { get; init; }
        
        public string Language { get; init; }
    }