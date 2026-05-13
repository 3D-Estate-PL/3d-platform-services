using System.Linq.Expressions;
using InteriorDesigners.Domain.InteriorDesigner;

namespace InteriorDesigners.Application.Services;

public interface IInteriorDesignerProductsRepository
{
    Task<ProductAggregate?> FindAsync(ProductIdentity productIdentity);
    Task UpsertAsync(ProductAggregate product, CancellationToken cancellationToken);
    Task<List<ProductAggregate>> FindAll(Expression<Func<ProductAggregate, bool>> predicate);
}