using BuildingBlocks.Abstractions.CQRS.CQRS;
using BuildingBlocks.Infrastructure.DataAccess.Cosmos.Extensions;
using CoreProducts.Application.Styles.Dtos;
using CoreProducts.Application.Styles.Queries;
using CoreProducts.Domain.DefaultProducts;
using CoreProducts.Infrastructure.DataAccess;

namespace CoreProducts.Infrastructure.Queries.StylesDefaultProducts;

public class
    GetStyleDefaultProductsQueryHandler : IQueryHandler<GetStyleDefaultProductsQuery, GetStyleDefaultProductsResponse>
{
    private readonly CoreProductsDbContext _dbContext;


    public GetStyleDefaultProductsQueryHandler(CoreProductsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetStyleDefaultProductsResponse> Handle(GetStyleDefaultProductsQuery request,
        CancellationToken cancellationToken)
    {
        var documents = await _dbContext.Query<StyleDefaultProducts>().ToDocumentListAsync();

        return new GetStyleDefaultProductsResponse
        {
            Items = documents.Result.Select(x =>
            {
                return new StyleDefaultProductsDto
                {
                    Code = x.Code,
                    RoomTypes = x.RoomTypes.Select(y => new RoomDto
                    {
                        Type = y.Type,
                        DefaultProducts = y.DefaultProducts.Select(z => new DefaultProductDto
                        {
                            CategoryName = z.CategoryName,
                            ProductId = z.ProductId
                        }).ToList()
                    }).ToList()
                };
            }).ToList()
        };
    }
}