using Azure.Storage.Blobs;

namespace ImagesManagement.Application.Services;

public interface IBlobStorageClientProvider
{
    public BlobServiceClient GetBlobServiceClient(string connection);
}