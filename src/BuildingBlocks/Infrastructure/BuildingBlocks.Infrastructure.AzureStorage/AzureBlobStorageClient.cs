using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BuildingBlocks.Abstractions.Storage;

namespace BuildingBlocks.Infrastructure.AzureStorage;

public class AzureBlobStorageClient : IBlobFileProvider
{
    private readonly BlobServiceClient _blobClient;
    public AzureBlobStorageClient(BlobServiceClient blobServiceClient)
    {
        _blobClient = blobServiceClient;
    }

    public async Task<Stream> OpenReadAsync(string fileUrl)
    {
        var blob = new BlobClient(new Uri(fileUrl));
        var fileName = blob.Name;
        var containerName = blob.BlobContainerName;
        
        var blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
        var blobClient = blobContainerClient.GetBlobClient(fileName);
        return await blobClient.OpenReadAsync();
    }

    public async Task MoveAsync(string fileUrl, string folder, string suffixFile)
    {
        var blob = new BlobClient(new Uri(fileUrl));
        var fileName = blob.Name;
        var containerName = blob.BlobContainerName;
        
        var blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
        var blobClient = blobContainerClient.GetBlobClient(fileName);


        var destinationBlobFileName = $"{folder}/{Path.GetFileName(fileName)}.{suffixFile}";
        
        var destinationBlob = blobContainerClient.GetBlobClient(destinationBlobFileName);
        await destinationBlob.StartCopyFromUriAsync(new Uri(fileUrl));
        await blobClient.DeleteAsync();

    }
}