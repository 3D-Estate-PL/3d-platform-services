using BuildingBlocks.Abstractions.CQRS.CQRS;
using CoreProducts.Application.Styles.Dtos;

namespace CoreProducts.Application.Styles.Queries;

public class GetStyleDefaultProductsResponse
{
    public List<StyleDefaultProductsDto> Items { get; init; }
}

public class GetStyleDefaultProductsQuery : IQuery<GetStyleDefaultProductsResponse>
{
}