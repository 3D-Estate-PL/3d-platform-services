using CoreProducts.Application.Styles.Services;
using CoreProducts.Domain.DefaultProducts;

namespace CoreProducts.Infrastructure.DataAccess.StyleDefaultProducts;

public class StyleDefaultProductsRepository : IStyleDefaultProductsRepository
{
    private readonly CoreProductsDbContext _dbContext;

    public StyleDefaultProductsRepository(CoreProductsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Domain.DefaultProducts.StyleDefaultProducts?> FindAsync(string id)
    {
        return await _dbContext.FindAsync<Domain.DefaultProducts.StyleDefaultProducts>(new StyleDefaultProductsIdentity(id));
    }

    public async Task UpsertAsync(Domain.DefaultProducts.StyleDefaultProducts product, CancellationToken cancellationToken)
    {
        await _dbContext.UpsertAsync(product);
    }

   
}