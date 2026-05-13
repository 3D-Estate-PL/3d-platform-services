using _3dEstate.Platform.Services.InteriorDesigners.IntegrationEvents.Events;
using BuildingBlocks.Application.EventBus.Events;

namespace InteriorDesigners.Application.IntegrationEvents.Events;

[TopicName("CoreProductsUpdated")]
public record CoreProductsUpdatedIntegrationEvent : IntegrationEvent
{
    
}
    
