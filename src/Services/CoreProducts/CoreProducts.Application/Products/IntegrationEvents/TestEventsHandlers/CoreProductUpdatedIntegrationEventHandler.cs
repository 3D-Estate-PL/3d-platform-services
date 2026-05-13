using BuildingBlocks.Application.EventBus.Events;
using CoreProducts.Application.Products.IntegrationEvents.Events;
using CoreProducts.Application.Products.IntegrationEvents.Events.External;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;

namespace CoreProducts.Application.Products.IntegrationEvents.TestEventsHandlers;

public class CoreProductUpdatedIntegrationEventHandler : IntegrationEventHandler<CoreProductsUpdatedIntegrationEvent>

{
    private readonly ILogger<CoreProductUpdatedIntegrationEventHandler> _logger;


    public CoreProductUpdatedIntegrationEventHandler(ILoggerFactory loggerFactory, 
        ILogger<CoreProductUpdatedIntegrationEventHandler> logger, Tracer tracer) : 
        base(loggerFactory, tracer)
    {
        _logger = logger;
    }

    protected override Task HandleEvent(CoreProductsUpdatedIntegrationEvent @event, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}