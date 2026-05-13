using Azure.Storage.Blobs;
using ImagesManagement.Domain;

namespace ImagesManagement.Application.Services;

public interface IImageResizer
{
    Task Resize(string container, string sourceFileName, 
        ImageResolutionEntity imageResolutionEntity,
        BlobServiceClient blobServiceClient, string? destinationFileName);
}