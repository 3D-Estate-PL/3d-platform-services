using BuildingBlocks.Application.EventBus.Events;

namespace BuildingBlocks.Application.EventBus;

public interface IEventBus
{
    Task PublishAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent)
        where TIntegrationEvent : IntegrationEvent;
}