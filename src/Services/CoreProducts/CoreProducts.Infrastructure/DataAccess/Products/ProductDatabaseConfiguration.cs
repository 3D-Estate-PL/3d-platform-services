using BuildingBlocks.Infrastructure.DataAccess.Cosmos;

namespace CoreProducts.Infrastructure.DataAccess.Products;

public record ProductDatabaseConfiguration : CosmosConfiguration
{
    public override string ConfigurationName => "ProductDatabase";
}