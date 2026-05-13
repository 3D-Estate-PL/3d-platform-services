using ImagesManagement.Domain;

namespace ImagesManagement.Application.Images;

public interface IImageMetadataRepository
{
    Task<ImageMetadataAggregate> GetAsync(string id, string partitionKey);
    Task UpsertAsync(ImageMetadataAggregate imageMetadata);
    Task<ImageMetadataAggregate?> FindWithPartitionKeyFilter(string partitionKey, string id);
}