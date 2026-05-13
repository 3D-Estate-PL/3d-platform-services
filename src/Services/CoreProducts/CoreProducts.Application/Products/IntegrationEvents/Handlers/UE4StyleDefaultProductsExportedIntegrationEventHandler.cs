using BuildingBlocks.Application.EventBus.Events;
using CoreProducts.Application.Products.IntegrationEvents.Events;
using CoreProducts.Application.Products.IntegrationEvents.Events.Internal;
using CoreProducts.Application.Styles.Commands.ImportDefaultProductsForStyles;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;

namespace CoreProducts.Application.Products.IntegrationEvents.Handlers;

// ReSharper disable once InconsistentNaming
public class UE4StyleDefaultProductsExportedIntegrationEventHandler : IntegrationEventHandler<
        UE4StyleDefaultProductsExportedIntegrationEvent>
{
    private readonly IMediator _mediator;

    public UE4StyleDefaultProductsExportedIntegrationEventHandler(ILoggerFactory loggerFactory, 
        IMediator mediator, Tracer tracer) : 
        base(loggerFactory, tracer)
    {
        _mediator = mediator;
    }

    protected override async Task HandleEvent(UE4StyleDefaultProductsExportedIntegrationEvent @event,
        CancellationToken cancellationToken)
    {
        var blobFileUrl = @event.Url;
        await _mediator.Send(new ImportDefaultStyleProductsFromFileCommand(blobFileUrl));
    }
}