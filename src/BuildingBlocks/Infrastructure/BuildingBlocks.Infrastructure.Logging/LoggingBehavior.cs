using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Infrastructure.Logging;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }


    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        const string prefix = nameof(LoggingBehavior<TRequest, TResponse>);

        _logger.LogInformation("[{@CommandHandler}] Handle request={@CommandName} and response={@CommandResponse}",
            prefix, typeof(TRequest).DeclaringType?.Name, typeof(TResponse).DeclaringType?.Name);

        var timer = new Stopwatch();
        timer.Start();

        var response = await next();

        timer.Stop();
        var timeTaken = timer.Elapsed;
        if (timeTaken.Seconds > 3) // if the request is greater than 3 seconds, then log the warnings
            _logger.LogWarning("[{@CommandHandler}] The request {@CommandName} took {@TimeTaken} seconds.",
                prefix, typeof(TRequest).DeclaringType?.Name, timeTaken.Seconds);

        _logger.LogDebug("[{@CommandHandler}] Handled {@CommandName}", prefix,
            typeof(TRequest).DeclaringType?.Name);
        return response;
    }
}