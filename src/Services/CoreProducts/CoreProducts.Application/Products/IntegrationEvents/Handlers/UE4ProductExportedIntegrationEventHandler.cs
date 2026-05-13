using BuildingBlocks.Application.EventBus.Events;
using CoreProducts.Application.Products.Commands;
using CoreProducts.Application.Products.IntegrationEvents.Events;
using CoreProducts.Application.Products.IntegrationEvents.Events.Internal;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;

namespace CoreProducts.Application.Products.IntegrationEvents.Handlers;

// ReSharper disable once InconsistentNaming
public class UE4ProductExportedIntegrationEventHandler : IntegrationEventHandler<UE4ProductExportedIntegrationEvent>
{
    private readonly IMediator _mediator;
    
    public UE4ProductExportedIntegrationEventHandler(ILoggerFactory loggerFactory, 
        IMediator mediator, Tracer tracer) : base(loggerFactory, tracer)
    {
        _mediator = mediator;
    }

    protected override async Task HandleEvent(UE4ProductExportedIntegrationEvent @event, CancellationToken cancellationToken)
    {
        var blobFileUrl = @event.Url;
        await _mediator.Send(new ImportProductsFromFileCommand(blobFileUrl));
    }
}