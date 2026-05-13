using BuildingBlocks.Application.EventBus.Events;

namespace CoreProducts.Application.Products.IntegrationEvents.Events.External;

[TopicName("CoreProductsUpdated")]
public record CoreProductsUpdatedIntegrationEvent : IntegrationEvent
{
    
}