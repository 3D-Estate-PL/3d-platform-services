using BuildingBlocks.Infrastructure.DataAccess.Cosmos;
using ImagesManagement.Domain;

namespace ImagesManagement.Infrastructure.DataAccess;

public class ImagesDbContext : CosmosContext
{
    public ImagesDbContext(CosmosConfiguration configuration) : base(configuration)
    {
        Map<ImageMetadataAggregate>("images-metadata");
    }
}