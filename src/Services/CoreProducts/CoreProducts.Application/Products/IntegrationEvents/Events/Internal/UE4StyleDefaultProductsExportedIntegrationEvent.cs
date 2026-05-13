using BuildingBlocks.Application.EventBus.Events;

namespace CoreProducts.Application.Products.IntegrationEvents.Events.Internal;

[TopicName("UE4StyleDefaultProductsExported")]
public record UE4StyleDefaultProductsExportedIntegrationEvent : BlobIntegrationEvent
{
    
}