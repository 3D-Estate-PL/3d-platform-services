using BuildingBlocks.Application.EventBus;
using BuildingBlocks.Application.EventBus.Events;
using Dapr.Client;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Abstractions.EventBus.Dapr.Dapr;

internal class DaprEventBus : IEventBus
{
    public const string PubSubName = "pubsub";

    private readonly DaprClient _dapr;
    private readonly ILogger _logger;

    public DaprEventBus(DaprClient dapr, ILogger<DaprEventBus> logger)
    {
        _dapr = dapr;
        _logger = logger;
    }

    public async Task PublishAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, string sessionId)
        where TIntegrationEvent : IntegrationEvent
    {
        var topicName = IntegrationEvent.GetTopicName(integrationEvent);

        _logger.LogInformation(
            "Publishing event {Event} to {PubsubName}.{TopicName}",
            integrationEvent,
            PubSubName,
            topicName);
        
        var metadata = new Dictionary<string, string>
        {
            { "SessionId", sessionId }
        };

        // We need to make sure that we pass the concrete type to PublishEventAsync,
        // which can be accomplished by casting the event to dynamic. This ensures
        // that all event fields are properly serialized.
        await _dapr.PublishEventAsync(PubSubName, topicName, (object) integrationEvent, metadata);
    }
}