using BuildingBlocks.Abstractions.CQRS.CQRS;
using BuildingBlocks.Infrastructure.DataAccess.Cosmos.Extensions;
using CoreProducts.Application.Products.Dtos;
using CoreProducts.Application.Products.Queries.GetProducts;
using CoreProducts.Domain.Products;
using CoreProducts.Infrastructure.DataAccess;

namespace CoreProducts.Infrastructure.Queries;

internal class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, GetProductsResponse>
{
    private readonly CoreProductsDbContext _dbContext;

    public GetProductsQueryHandler(CoreProductsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetProductsResponse> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var ids = request.Ids;

        List<Product> products;

        if (ids != null && ids.Any())
        {
            var documents = await _dbContext.Query<Product>().Where(x => ids.Contains(x.Id) && 
                                                                      x.IsDeleted != false).ToDocumentListAsync();

            products = documents.Result;
        }
        else
        {
            var documents = await _dbContext.Query<Product>().ToDocumentListAsync();
            products = documents.Result;
        }

        var productsModel = products.Select(product => new CoreProductDto
            {
                Id = product.Id,
                Category = new CategoryDto
                {
                    Name = product.Category.Type,
                    DisplayName = product.Category.DisplayName
                },
                Unit = product.Unit,
                Categories = product.Categories,
                Name = product.Name,
                SubCategories = product.SubCategories,
                ThumbnailUrl = $"TODO",
                Designers = product.Designers
            })
            .ToList();

        return new GetProductsResponse(productsModel);
    }
}
