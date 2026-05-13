using MediatR;

namespace BuildingBlocks.Application.EventBus.Events;

public abstract record BlobIntegrationEvent : IntegrationEvent
{
    public string Api { get; set; }
    public string ContentType { get; set; }
    public string Url { get; set; }
}


public abstract record IntegrationEvent() : INotification
{
    public Guid Id { get; } = Guid.NewGuid();

    public DateTime CreationDate { get; } = DateTime.UtcNow;

    public static string GetTopicName<TIntegrationEvent>(TIntegrationEvent @event)  where TIntegrationEvent : IntegrationEvent
    {
        return GetTopicName(@event.GetType());
    }

    public static string GetTopicName<TIntegrationEvent>() where TIntegrationEvent : IntegrationEvent
    {
        return GetTopicName(typeof(TIntegrationEvent));
    }

    private static string GetTopicName(Type type) 
    {
        var topicNameAttribute =
            (TopicNameAttribute) Attribute.GetCustomAttribute(type, typeof(TopicNameAttribute))!;

        if (topicNameAttribute == null)
        {
            throw new ArgumentException($"Topic Attribute not found. EventType: {type.Name}");
        }

        return topicNameAttribute.Name;
    }
}

public class TopicNameAttribute : Attribute
{
    public string Name { get; }

    public TopicNameAttribute(string name)
    {
        Name = name;
    }
}
