namespace TourStylesConfigurator.Api.Infrastructure.Database;

public class EntityNotFoundException : Exception
{
    private EntityNotFoundException(string entityType, string message)
    {
    }

    public static EntityNotFoundException New<TEntity>(string entityId)
    {
        return new EntityNotFoundException(typeof(TEntity).Name, entityId);
    }
}