using System.Linq.Expressions;
using BuildingBlocks.Infrastructure.DataAccess.Cosmos.Extensions;
using InteriorDesigners.Application.Services;
using InteriorDesigners.Domain.InteriorDesigner;

namespace InteriorDesigners.Infrastructure.DataAccess;

public class InteriorDesignerProductsRepository : IInteriorDesignerProductsRepository
{
    private readonly InteriorDesignerContext _interiorDesignerContext;

    public InteriorDesignerProductsRepository(InteriorDesignerContext interiorDesignerContext)
    {
        _interiorDesignerContext = interiorDesignerContext;
    }

    public async Task<ProductAggregate?> FindAsync(ProductIdentity productIdentity)
    {
        return await _interiorDesignerContext.FindAsync<ProductAggregate>(productIdentity);
    }

    public async Task UpsertAsync(ProductAggregate product, CancellationToken cancellationToken)
    {
        await _interiorDesignerContext.UpsertAsync(product);
    }

    public async Task<List<ProductAggregate>> FindAll(Expression<Func<ProductAggregate, bool>> predicate)
    {
        var result =  await _interiorDesignerContext.Query<ProductAggregate>()
            .Where(predicate).ToDocumentListAsync();
        
        return result.Result;
    }
}