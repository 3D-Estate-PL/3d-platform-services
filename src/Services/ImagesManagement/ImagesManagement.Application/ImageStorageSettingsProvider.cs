using ImagesManagement.Application.Images;

namespace ImagesManagement.Application;

public interface IImageStorageSettingsProvider
{
    Task<ImageStorageSettings> GetConfigurationAsync();
}