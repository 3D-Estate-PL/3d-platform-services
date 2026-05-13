using System.Text.Json.Nodes;
using Azure.Identity;
using Azure.Storage.Blobs;
using BuildingBlocks.Abstractions.EventBus.Dapr;
using BuildingBlocks.Infrastructure.AzureStorage;
using BuildingBlocks.Infrastructure.Cqrs.Mediatr;
using BuildingBlocks.Infrastructure.DataAccess.Cosmos;
using ImagesManagement.Application;
using ImagesManagement.Application.Images;
using ImagesManagement.Application.IntegrationEvents;
using ImagesManagement.Application.Services;
using ImagesManagement.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ImagesManagement.Infrastructure;

public static class ModuleInitializer
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        ConfigurationManager configuration, bool isLocal)
    {
        //CQS
        services.RegisterCqs(typeof(CommandsAssemblyMarker).Assembly,typeof(QueryAssemblyMarker).Assembly);
        
        var vault = configuration["KeyVaultName"];
        var managedIdentityClientId = configuration["ManagedIdentityClientId"];
        
        configuration.AddAzureKeyVault(new Uri($"https://{vault}.vault.azure.net/"),
            new DefaultAzureCredential(Options(isLocal,managedIdentityClientId)));
     
        services.AddCosmosContext<ImagesDbContext,ImageDatabaseConfiguration>(configuration);
        services.AddScoped<IImageMetadataRepository, ImageMetadataRepository>();
        services.AddScoped<IImageResizer, ImageResizer>();
        services.AddStorageClient(configuration,"ProductsStorageConfig");
        
        services.AddSingleton<IImageStorageSettingsProvider>( x => 
            new ImageStorageSettingsProvider(isLocal, managedIdentityClientId, configuration));

        services.AddSingleton<IBlobStorageClientProvider, BlobStorageClientProvider>();
        
        services.AddEventBus();
        services.AddHttpClient();

        return services;
    }

    public static DefaultAzureCredentialOptions Options(bool isLocal, string clientId)
    {
        if (string.IsNullOrEmpty(clientId))
        {
            clientId = null;
        }
        
        return new DefaultAzureCredentialOptions
        {
            ExcludeEnvironmentCredential = !isLocal,
            ExcludeInteractiveBrowserCredential = true,
            ExcludeAzurePowerShellCredential = true,
            ExcludeSharedTokenCacheCredential = true,
            ExcludeVisualStudioCodeCredential = true,
            ExcludeVisualStudioCredential = true,
            ExcludeWorkloadIdentityCredential = isLocal,
            ExcludeAzureCliCredential = isLocal,
            ExcludeAzureDeveloperCliCredential = isLocal,
            ExcludeManagedIdentityCredential = isLocal,
            ManagedIdentityClientId = clientId
        };
    }
    
    public static IApplicationBuilder RegisterIntegrationEventSubscriptions(this WebApplication app)
    {
        app
            .AddSubscription<ResizeImageRequested>()
            .Build();

        return app;
    }

}