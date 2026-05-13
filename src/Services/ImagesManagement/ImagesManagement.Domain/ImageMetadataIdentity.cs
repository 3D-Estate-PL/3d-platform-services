using BuildingBlocks.Domain.DDD;

namespace ImagesManagement.Domain;

public class ImageMetadataIdentity : DocumentIdentity<ImageMetadataAggregate>
{
    public ImageMetadataIdentity(string id, string partitionKey) : base(id)
    {
        PartitionKey = partitionKey;
        DocumentId = Id;
    }

    public override string PartitionKey { get; }
    public override string DocumentId { get; }
}