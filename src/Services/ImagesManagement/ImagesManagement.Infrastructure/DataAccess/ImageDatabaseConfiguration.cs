using BuildingBlocks.Infrastructure.DataAccess.Cosmos;

namespace ImagesManagement.Infrastructure.DataAccess;

public record ImageDatabaseConfiguration : CosmosConfiguration
{
    public override string ConfigurationName => "ImageDatabaseConfig";
}