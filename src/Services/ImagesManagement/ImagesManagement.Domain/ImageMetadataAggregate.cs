using System.Text.Json;
using System.Text.Json.Serialization;
using BuildingBlocks.Domain.DDD;
using Newtonsoft.Json;

namespace ImagesManagement.Domain;

public class ImageMetadataAggregate : IDocument
{
    [JsonProperty()]
    public string Container { get; private set; }

    public DateTimeOffset? LastModified { get;  set;}

    [JsonProperty()]
    public string Id { get; private set; }


    private List<ImageResolutionEntity> _resolutions;
    [System.Text.Json.Serialization.JsonIgnore]
    public IEnumerable<ImageResolutionEntity> Resolutions => _resolutions;
    
    public ImageMetadataAggregate()
    {
        _resolutions = new List<ImageResolutionEntity>();
    }

    public ImageMetadataAggregate(string container, string id, DateTimeOffset createdDateTime) : this()
    {
        Container = container;
        Id = id;
        LastModified = createdDateTime;
    }


    public void AddResolution(ImageResolutionEntity imageResolutionEntity)
    {
        var existsResolutionWithDifferentCompression = _resolutions.SingleOrDefault(x =>
            x.Width == imageResolutionEntity.Width
            && x.Height == imageResolutionEntity.Height);
        
        if (existsResolutionWithDifferentCompression != null)
        {
            _resolutions.Remove(existsResolutionWithDifferentCompression);
        }
        
        if (HasResolution(imageResolutionEntity) == false)
        {
            _resolutions.Add(imageResolutionEntity);
        }
    }

    public bool HasResolution(ImageResolutionEntity imageResolutionEntity)
    {
        return _resolutions.Contains(imageResolutionEntity);
    }

    public DocumentIdentity GetIdentity()
    {
        return new ImageMetadataIdentity(Id,Container);
    }

    public void Reset(DateTimeOffset originalFileModifiedDate)
    {
        LastModified = originalFileModifiedDate;
        _resolutions.Clear();
    }
}