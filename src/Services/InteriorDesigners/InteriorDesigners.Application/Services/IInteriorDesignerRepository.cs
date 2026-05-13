using System.Collections;
using InteriorDesigners.Domain.InteriorDesigner;

namespace InteriorDesigners.Application.Services;

public interface IInteriorDesignerRepository
{
    Task<InteriorDesignerAggregate?> FindAsync(string code);
    Task UpsertAsync(InteriorDesignerAggregate product, CancellationToken cancellationToken);
    Task<List<InteriorDesignerAggregate>> GetAllAsync();
    Task DeleteAsync(InteriorDesignerAggregate interiorDesigner);
}