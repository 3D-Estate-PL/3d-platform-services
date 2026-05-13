using BuildingBlocks.Infrastructure.DataAccess.Cosmos;

namespace InteriorDesigners.Infrastructure.DataAccess;

public record InteriorDesignerDatabaseConfiguration : CosmosConfiguration
{
    public override string ConfigurationName => "InteriorDesignerDatabase";
}