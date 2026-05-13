using CoreProducts.Application.Products.Services;
using CoreProducts.Domain.Products;

namespace CoreProducts.Infrastructure.DataAccess.Products;

public class ProductRepository : IProductRepository
{
    private readonly CoreProductsDbContext _dbContext;

    public ProductRepository(CoreProductsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Product?> FindAsync(string id)
    {
        return await _dbContext.FindAsync<Product>(new CoreProductIdentity(id));
    }

    public async Task UpsertAsync(Product product, CancellationToken cancellationToken)
    {
        await _dbContext.UpsertAsync(product);
    }
}