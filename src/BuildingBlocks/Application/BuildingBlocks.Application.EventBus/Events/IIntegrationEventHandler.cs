using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;

namespace BuildingBlocks.Application.EventBus.Events;

public abstract class IntegrationEventHandler<TIntegrationEvent> : INotificationHandler<TIntegrationEvent>
    where TIntegrationEvent : IntegrationEvent
{
    private readonly ILogger<IntegrationEventHandler<TIntegrationEvent>> _logger;
    private readonly Tracer _tracer;

    protected IntegrationEventHandler(ILoggerFactory loggerFactory, Tracer tracer)
    {
        _tracer = tracer;
        _logger = loggerFactory.CreateLogger<IntegrationEventHandler<TIntegrationEvent>>();
    }

    protected abstract Task HandleEvent(TIntegrationEvent @event, CancellationToken cancellationToken);

    public async Task Handle(TIntegrationEvent notification, CancellationToken cancellationToken)
    {
        using var span = _tracer.StartActiveSpan("Integration Event");
        try
        {
            var prefix = GetType().Name;

            _logger.LogInformation("Executing [{IntegrationHandler}] handler for event={IntegrationEvent}",
                prefix, typeof(TIntegrationEvent).Name);

            _logger.LogInformation("Event Body: [{EventBody}]", notification);

            var timer = new Stopwatch();
            timer.Start();

            await HandleEvent(notification, cancellationToken);

            timer.Stop();
            var timeTaken = timer.Elapsed;
            if (timeTaken.Seconds > 3) // if the request is greater than 3 seconds, then log the warnings
                _logger.LogWarning(
                    "[{IntegrationHandler}] The IntegrationEvent {IntegrationEvent} took {TimeTaken} seconds.",
                    prefix, typeof(TIntegrationEvent).Name, timeTaken.Seconds);

            _logger.LogDebug("Executed [{IntegrationHandler}] handler  for event={IntegrationEvent} completed.", prefix,
                typeof(TIntegrationEvent).Name);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e);
            span.SetStatus(Status.Error);
            span.RecordException(e);
            throw;
        }
    }
}