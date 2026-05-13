using BuildingBlocks.Abstractions.EventBus.Dapr.Dapr;
using BuildingBlocks.Application.EventBus.Events;
using BuildingBlocks.Application.EventBus.Subscriptions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Abstractions.EventBus.Dapr;

internal class SubscriptionBuilder : ISubscriptionBuilder
{
    private readonly List<DaprSubscription> _daprSubscriptions = new();
    private readonly WebApplication _webApplication;

    public SubscriptionBuilder(WebApplication webApplication)
    {
        _webApplication = webApplication;
    }

    public ISubscriptionBuilder AddSubscription<TIntegrationEvent>(bool requiredSession = false)
    where TIntegrationEvent : IntegrationEvent
    {
        var eventName = IntegrationEvent.GetTopicName<TIntegrationEvent>();
        var route = "/"+eventName;

        var subscription = new DaprSubscription
        {
            SubscriptionMetadata = new SubscriptionMetadata{ RequireSessions = requiredSession ? "true" : "false"},
            PubSubName = DaprEventBus.PubSubName,
            Topic = eventName,
            Route = route
        };

        _daprSubscriptions.Add(subscription);

        _webApplication.MapPost(route,
            async (IMediator mediator, TIntegrationEvent @event, CancellationToken cancellation) =>
            { await mediator.Publish(@event, cancellation).ConfigureAwait(false); });

        return this;
    }

    public void Build()
    {
        _webApplication.MapGet("/dapr/subscribe", async (ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("General");
            _daprSubscriptions.ForEach(x =>
            {
                logger.LogInformation("Register Subscription for {RegisteredSubscription}", x);

            });
            
            await Task.CompletedTask;
            return Results.Json(_daprSubscriptions);
        });
    }
}