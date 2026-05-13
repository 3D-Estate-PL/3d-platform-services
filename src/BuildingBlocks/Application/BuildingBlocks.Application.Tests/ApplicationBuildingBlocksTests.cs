using BuildingBlocks.Application.EventBus.Events;

namespace BuildingBlocks.Application.Tests;

public class ApplicationBuildingBlocksTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void get_topic_name_should_return_value_from_topic_name_attribute()
    {
        var topicName =  IntegrationEvent.GetTopicName<SampleIntegrationEvent>(); 
        Assert.That(topicName, Is.EqualTo("Test"));
    }


    [TopicName("Test")]
    public record SampleIntegrationEvent : IntegrationEvent
    {
        
    }
}