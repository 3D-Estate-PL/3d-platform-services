using CoreProducts.Domain.Products;

namespace CoreProducts.Application.Products.Services;

public interface IProductRepository
{
    Task<Product?> FindAsync(string id);
    Task UpsertAsync(Product product, CancellationToken cancellationToken);
}