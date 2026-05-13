using Azure.Identity;
using BuildingBlocks.Abstractions.EventBus.Dapr;
using BuildingBlocks.Infrastructure.AzureStorage;
using BuildingBlocks.Infrastructure.Cqrs.Mediatr;
using BuildingBlocks.Infrastructure.DataAccess.Cosmos;
using BuildingBlocks.Infrastructure.GoogleSheet;
using Dapr.Client;
using InteriorDesigners.Application;
using InteriorDesigners.Application.IntegrationEvents.Events;
using InteriorDesigners.Application.Services;
using InteriorDesigners.Infrastructure;
using InteriorDesigners.Infrastructure.DataAccess;
using InteriorDesigners.Infrastructure.Queries.Products;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ModuleInitializer
{
    public static IServiceCollection AddCoreProductLibrary(this IServiceCollection services, 
        ConfigurationManager configuration, bool isLocal)
    {
        //CQS
        services.RegisterCqs(typeof(AssemblyMarker).Assembly,typeof(QueryAssemblyMarker).Assembly);
        
        //Google Sheet Client
        services.RegisterGoogleSheetClient();
        
        
        
        var vault = configuration["KeyVaultName"];
        var managedIdentityClientId = configuration["ManagedIdentityClientId"];
        var options = new DefaultAzureCredentialOptions
        {
            ManagedIdentityClientId = string.IsNullOrEmpty(managedIdentityClientId) ? null: managedIdentityClientId
        };
        
        configuration.AddAzureKeyVault(new Uri($"https://{vault}.vault.azure.net/"),
            new DefaultAzureCredential(options));
        
        services.AddSingleton<ConfigurationsSettings>(x => new ConfigurationsSettings
        {
            ConfigurationsSheetId = configuration["ConfigurationsSheetId"]
        });

        services.AddCosmosContext<InteriorDesignerContext,InteriorDesignerDatabaseConfiguration>(configuration);
        services.AddScoped<IInteriorDesignerRepository, InteriorDesignerRepository>();
        services.AddScoped<IInteriorDesignerProductsRepository, InteriorDesignerProductsRepository>();
        services.AddScoped<IProductSubCategoriesService, ProductSubCategoriesService>();
        
        services.AddMemoryCache();
        services.AddHttpClient();
        services.AddStorageClient(configuration,"ProductsStorageConfig");

        services.AddSingleton<ICoreProductsServiceClient, CoreProductsServiceClient>(
            _ => new CoreProductsServiceClient(DaprClient.CreateInvokeHttpClient("core-products-api")));

        
        services.AddSingleton(x =>
        {
            var serviceConfiguration = new ImageStorageSettings();
            var configuration = x.GetRequiredService<IConfiguration>();
            serviceConfiguration.ImageServiceUrl = configuration["ImageServiceUrl"];
            return serviceConfiguration;

        });
        
        
        return services;
    }

    public static IApplicationBuilder RegisterIntegrationEventSubscriptions(this WebApplication app)
    {
        app
            .AddSubscription<CoreProductsUpdatedIntegrationEvent>()
            .Build();

        return app;
    }
    
    
    private static DefaultAzureCredentialOptions Options(bool isLocal, string clientId)
    {
        if (string.IsNullOrEmpty(clientId))
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