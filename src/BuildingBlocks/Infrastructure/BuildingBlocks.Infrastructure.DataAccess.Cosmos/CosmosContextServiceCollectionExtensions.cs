using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BuildingBlocks.Infrastructure.DataAccess.Cosmos
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public interface ICosmosConfiguration
    {
        public string ConnectionString { get; }
        public string DatabaseName { get; }
        public string Region { get; }
        
        public string ConfigurationName { get; }
    }


    public static class CosmosContextServiceCollectionExtensions
    {
        public static IServiceCollection AddCosmosContext<TDbContext,TConfiguration>(this IServiceCollection services, 
            IConfiguration configuration) where TDbContext : CosmosContext
        where TConfiguration : ICosmosConfiguration, new()
        {
            
            var serviceConfiguration = new TConfiguration();
            configuration.Bind(serviceConfiguration.ConfigurationName, serviceConfiguration);
            
            services.TryAddSingleton<TDbContext>(provider =>
            {
                var cosmosContext = (TDbContext)Activator.CreateInstance(typeof(TDbContext), serviceConfiguration)!;
                return cosmosContext;
            });

            return services;
        }
    }
}