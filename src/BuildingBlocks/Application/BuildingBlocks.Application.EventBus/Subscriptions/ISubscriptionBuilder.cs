using BuildingBlocks.Application.EventBus.Events;

namespace BuildingBlocks.Application.EventBus.Subscriptions;

public interface ISubscriptionBuilder
{
    ISubscriptionBuilder AddSubscription<TIntegrationEvent>(bool requiredSession=false)
        where TIntegrationEvent : IntegrationEvent;
    void Build();
}