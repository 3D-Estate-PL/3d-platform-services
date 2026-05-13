using CoreProducts.Domain.DefaultProducts;

namespace CoreProducts.Application.Styles.Services;

public interface IStyleDefaultProductsRepository
{
    Task<StyleDefaultProducts?> FindAsync(string id);
    Task UpsertAsync(StyleDefaultProducts product, CancellationToken cancellationToken);
}