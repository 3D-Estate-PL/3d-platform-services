using Azure.Identity;
using BuildingBlocks.Abstractions.EventBus.Dapr;
using BuildingBlocks.Infrastructure.AzureStorage;
using BuildingBlocks.Infrastructure.Cqrs.Mediatr;
using BuildingBlocks.Infrastructure.DataAccess.Cosmos;
using BuildingBlocks.Infrastructure.GoogleSheet;
using CoreProducts.Application;
using CoreProducts.Application.Products.IntegrationEvents.Events;
using CoreProducts.Application.Products.IntegrationEvents.Events.External;
using CoreProducts.Application.Products.IntegrationEvents.Events.Internal;
using CoreProducts.Application.Products.Services;
using CoreProducts.Application.Styles.Services;
using CoreProducts.Infrastructure.DataAccess;
using CoreProducts.Infrastructure.DataAccess.Products;
using CoreProducts.Infrastructure.DataAccess.StyleDefaultProducts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreProducts.Infrastructure;

public static class ModuleInitializer
{
    public static IServiceCollection AddCoreProductLibrary(this IServiceCollection services,
        ConfigurationManager configuration, bool isLocal)
    {
        //CQS
        services.RegisterCqs(typeof(CommandsAssemblyMarker).Assembly,typeof(QueryAssemblyMarker).Assembly);
        
        //Google Sheet Client
        services.RegisterGoogleSheetClient();
        
        
        var vault = configuration["KeyVaultName"];
        var managedIdentityClientId = configuration["ManagedIdentityClientId"];

        configuration.AddAzureKeyVault(new Uri($"https://{vault}.vault.azure.net/"),
            new DefaultAzureCredential(Options(isLocal,managedIdentityClientId)));
            
            
        services.AddSingleton<ConfigurationsSettings>(x => new ConfigurationsSettings
        {
            ConfigurationsSheetId = configuration["ConfigurationsSheetId"]
        });
        
        services.AddScoped<IRoomTypesProvider, RoomTypesProvider>();
        services.AddScoped<ICategoryProvider, CategoryProvider>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddCosmosContext<CoreProductsDbContext,ProductDatabaseConfiguration>(configuration);

        services.AddScoped<IStyleDefaultProductsRepository, StyleDefaultProductsRepository>();

        services.AddHttpClient();
        services.AddEventBus();
        services.AddStorageClient(configuration,"ProductsStorageConfig");
        
        return services;
    }

    public static IApplicationBuilder RegisterIntegrationEventSubscriptions(this WebApplication app)
    {
        app
            .AddSubscription<CoreProductsUpdatedIntegrationEvent>()
            .AddSubscription<UE4ProductExportedIntegrationEvent>()
            .AddSubscription<UE4StyleDefaultProductsExportedIntegrationEvent>()
            .Build();

        return app;
    }
    
    private static DefaultAzureCredentialOptions Options(bool isLocal, string clientId)
    {
        if (string.IsNullOrEmpty(clientId) || isLocal)
        {
            clientId = null;
        }
        
        return new DefaultAzureCredentialOptions
        {
            ExcludeEnvironmentCredential = true,
            ExcludeInteractiveBrowserCredential = true,
            ExcludeAzurePowerShellCredential = true,
            ExcludeSharedTokenCacheCredential = true,
            ExcludeVisualStudioCodeCredential = true,
            ExcludeVisualStudioCredential = true,
            ExcludeAzureCliCredential = !isLocal,
            ExcludeManagedIdentityCredential = isLocal,
            ManagedIdentityClientId = clientId
        };
    }
    
}