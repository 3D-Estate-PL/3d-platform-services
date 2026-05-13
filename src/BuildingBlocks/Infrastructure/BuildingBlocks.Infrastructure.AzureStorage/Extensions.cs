using Azure.Storage.Blobs;
using BuildingBlocks.Abstractions.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure.AzureStorage;

public static class Extensions
{
    public static IServiceCollection AddStorageClient(this IServiceCollection services, IConfiguration configuration, string configurationName)
    {
        var serviceConfiguration = new AzureBlobStorageSettings();
        configuration.Bind(configurationName, serviceConfiguration);
        services.AddSingleton<BlobServiceClient>(x => new BlobServiceClient(serviceConfiguration.ConnectionString));
        services.AddSingleton<IBlobFileProvider,AzureBlobStorageClient>();
        return services;
    }
}