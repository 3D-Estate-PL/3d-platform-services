using System.Text.Json.Serialization;

namespace BuildingBlocks.Abstractions.EventBus.Dapr.Dapr;

public record DaprSubscription
{
    [property: JsonPropertyName("pubsubname")]
    public required string PubSubName { get; init; }

    [property: JsonPropertyName("topic")] 
    public required string Topic { get; init; }

    [property: JsonPropertyName("route")] 
    public required string Route { get; init; }
    
    [property: JsonPropertyName("metadata")] 
    public required SubscriptionMetadata SubscriptionMetadata { get; init; }

    public override string ToString()
    {
        return $"PubSub:{PubSubName} | Topic:{Topic} | Route:{Route}";
    }
}

public record SubscriptionMetadata
{
    [property: JsonPropertyName("requireSessions")] 
    public required string RequireSessions { get; init; }
}