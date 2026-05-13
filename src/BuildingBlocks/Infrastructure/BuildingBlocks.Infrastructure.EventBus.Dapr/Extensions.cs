using BuildingBlocks.Abstractions.EventBus.Dapr.Dapr;
using BuildingBlocks.Application.EventBus;
using BuildingBlocks.Application.EventBus.Events;
using BuildingBlocks.Application.EventBus.Subscriptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Abstractions.EventBus.Dapr;

public static class Extensions
{
    private static SubscriptionBuilder? _subscriptionBuilder;

    public static IServiceCollection AddEventBus(this IServiceCollection service)
    {
        service.AddDaprClient();
        service.AddScoped<IEventBus, DaprEventBus>();
        return service;
    }

    public static ISubscriptionBuilder AddSubscription<TIntegrationEvent>(this WebApplication app, bool requiredSession = false)
    where TIntegrationEvent : IntegrationEvent
    {
        _subscriptionBuilder ??= new SubscriptionBuilder(app);
        _subscriptionBuilder.AddSubscription<TIntegrationEvent>(requiredSession);
        return _subscriptionBuilder;
    }
}