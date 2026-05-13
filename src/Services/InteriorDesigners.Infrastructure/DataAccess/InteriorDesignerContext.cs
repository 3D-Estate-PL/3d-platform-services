using BuildingBlocks.Infrastructure.DataAccess.Cosmos;
using InteriorDesigners.Domain.InteriorDesigner;

namespace InteriorDesigners.Infrastructure.DataAccess;

public class InteriorDesignerContext : CosmosContext
{
    public InteriorDesignerContext(CosmosConfiguration configuration) : base(configuration)
    {
        Map<InteriorDesignerAggregate>("interiorsDesigners-interiorsDesigners");
        Map<ProductAggregate>("interiorsDesigners-products");
    }
}