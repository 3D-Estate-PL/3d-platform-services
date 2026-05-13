using BuildingBlocks.Infrastructure.DataAccess.Cosmos.Extensions;
using BuildingBlocks.Infrastructure.DataAccess.Cosmos.Model;
using InteriorDesigners.Application.Services;
using InteriorDesigners.Domain.InteriorDesigner;

namespace InteriorDesigners.Infrastructure.DataAccess;

public class InteriorDesignerRepository : IInteriorDesignerRepository
{
    private readonly InteriorDesignerContext _interiorDesignerContext;

    public InteriorDesignerRepository(InteriorDesignerContext interiorDesignerContext)
    {
        _interiorDesignerContext = interiorDesignerContext;
    }

    public async Task<InteriorDesignerAggregate?> FindAsync(string id)
    {
        return await _interiorDesignerContext.FindAsync<InteriorDesignerAggregate>(new InteriorDesignerIdentity(id));
    }

    public async Task UpsertAsync(InteriorDesignerAggregate product, CancellationToken cancellationToken)
    {
         await _interiorDesignerContext.UpsertAsync(product);
    }

    public async Task<List<InteriorDesignerAggregate>> GetAllAsync()
    {
        var result = await _interiorDesignerContext.Query<InteriorDesignerAggregate>().ToDocumentListAsync();
        return result.Result.ToList();
    }

    public async Task DeleteAsync(InteriorDesignerAggregate interiorDesigner)
    {
       await _interiorDesignerContext.RemoveAsync(interiorDesigner);
    }
}