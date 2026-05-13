using Azure.Identity;
using Azure.Storage.Blobs;
using ImagesManagement.Application;
using ImagesManagement.Application.Images;
using Microsoft.Extensions.Configuration;

namespace ImagesManagement.Infrastructure;

public class ImageStorageSettingsProvider : IImageStorageSettingsProvider
{
    private readonly bool _isLocal;
    private readonly string _managedIdentityClientId;
    private readonly IConfiguration _configuration;
    private  ImageStorageSettings? _imageStorageSettings;
    public ImageStorageSettingsProvider(
        bool isLocal,
        string managedIdentityClientId, IConfiguration configuration)
    {
        _isLocal = isLocal;
        _managedIdentityClientId = managedIdentityClientId;
        _configuration = configuration;
    }

    public async Task<ImageStorageSettings> GetConfigurationAsync()
    {
        if (_imageStorageSettings == null)
        {
            var serviceConfiguration = new ImageStorageSettings();
            _configuration.Bind(nameof(ImageStorageSettings), serviceConfiguration);
            var fileUrl = serviceConfiguration.ConfigFileUrl;
            var bc = new BlobClient(new Uri(fileUrl), new DefaultAzureCredential(ModuleInitializer.Options(_isLocal, _managedIdentityClientId)));
            var content = await bc.DownloadAsync();
            serviceConfiguration = System.Text.Json.JsonSerializer.Deserialize<ImageStorageSettings>(content.Value.Content);
            _configuration.Bind(nameof(ImageStorageSettings), serviceConfiguration);
            _imageStorageSettings = serviceConfiguration;
        }
        
        return _imageStorageSettings;
    }
    

}