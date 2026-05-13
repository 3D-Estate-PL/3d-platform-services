using ImagesManagement.Application.Images;
using ImagesManagement.Domain;

namespace ImagesManagement.Infrastructure.DataAccess;

public class ImageMetadataRepository : IImageMetadataRepository
{
    private readonly ImagesDbContext _imagesDbContext;

    public ImageMetadataRepository(ImagesDbContext imagesDbContext)
    {
        _imagesDbContext = imagesDbContext;
    }

    public async Task<ImageMetadataAggregate> GetAsync(string id, string? partitionKey)
    {
        return await _imagesDbContext.GetAsync<ImageMetadataAggregate>(new ImageMetadataIdentity(id,partitionKey));
    }

    public async Task UpsertAsync(ImageMetadataAggregate imageMetadata)
    {
        await _imagesDbContext.UpsertAsync(imageMetadata);
    }

    public async Task<ImageMetadataAggregate?> FindWithPartitionKeyFilter(string partitionKey, string id)
    {
        return await _imagesDbContext.FindAsync<ImageMetadataAggregate>(new ImageMetadataIdentity(id,partitionKey));
    }
    
}