using System.Reflection;
using BuildingBlocks.Infrastructure.Logging;
using BuildingBlocks.Infrastructure.Validations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BuildingBlocks.Infrastructure.Cqrs.Mediatr;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterCqs(this IServiceCollection services,params Assembly[] assemblies)
    {
        services.AddMediatR(assemblies);
        services.TryAddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.TryAddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        return services;
    }
}