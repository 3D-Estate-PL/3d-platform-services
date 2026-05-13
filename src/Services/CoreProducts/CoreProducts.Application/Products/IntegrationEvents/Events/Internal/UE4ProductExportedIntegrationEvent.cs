using BuildingBlocks.Application.EventBus.Events;

namespace CoreProducts.Application.Products.IntegrationEvents.Events.Internal;

[TopicName("UE4ProductExported")]
public record UE4ProductExportedIntegrationEvent : BlobIntegrationEvent
{
    
}