using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure.DaprHealthChecks;

public static class DaprHealthCheckBuilderExtensions
{
    public static IHealthChecksBuilder AddDapr(this IHealthChecksBuilder builder) =>
        builder.AddCheck<DaprHealthCheck>("dapr");
}
