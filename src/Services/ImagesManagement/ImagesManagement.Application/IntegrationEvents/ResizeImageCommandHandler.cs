using System.Collections.Concurrent;
using BuildingBlocks.Application.EventBus;
using BuildingBlocks.Application.EventBus.Events;
using ImagesManagement.Application.Images;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;

namespace ImagesManagement.Application.IntegrationEvents;
public class ResizeImageCommandHandler : IntegrationEventHandler<ResizeImageRequested>

{
    private readonly ILogger<ResizeImageCommandHandler> _logger;
    private readonly IMediator _mediator;
    private readonly IEventBus _eventBus;
    public ResizeImageCommandHandler(ILoggerFactory loggerFactory, ILogger<ResizeImageCommandHandler> logger,
        IMediator mediator, Tracer tracer, IEventBus eventBus) :
        base(loggerFactory, tracer)
    {
        _logger = logger;
        _mediator = mediator;
        _eventBus = eventBus;
    }

    protected override async Task HandleEvent(ResizeImageRequested request,CancellationToken cancellationToken)
    {
       
        _logger.LogDebug("Start processing request.{RequestImageName}", request.ImageName);
        await NewMethod(request, cancellationToken);
        _logger.LogInformation("Completed processing request.{RequestImageName}", request.ImageName);
    }

    private async Task NewMethod(ResizeImageRequested request, CancellationToken cancellationToken)
    {
        var extension = Helper.GetExtension(request.DestinationFileName, request.ImageName);
        var fileEncoder = Helper.GetDefaultEncoder(Path.GetExtension(extension));
        var destinationFileName = Helper.GetDestinationFileName(request.DestinationFileName, fileEncoder);
 
        var command = new ResizeImageCommand(request.StorageKey, request.ImageName, request.Width,
            request.Height, request.MaxRes, fileEncoder, request.Compression ?? 90, destinationFileName, false);
        await _mediator.Send(command, cancellationToken);
    }
}