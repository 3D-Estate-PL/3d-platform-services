using BuildingBlocks.Application.EventBus.Events;

namespace ImagesManagement.Application.IntegrationEvents;

[TopicName("resize-image-requested")]
public record ResizeImageRequested : IntegrationEvent
{
    public string StorageKey { get; set; }
    public string ImageName{ get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? MaxRes { get; set; }
    public int? Compression { get; set; }
    public string? DestinationFileName { get; set; }
}

