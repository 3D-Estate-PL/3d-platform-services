using System.Collections.Concurrent;
using Azure.Storage.Blobs;
using ImagesManagement.Application.Services;

namespace ImagesManagement.Infrastructure;

public class BlobStorageClientProvider : IBlobStorageClientProvider
{
    private static readonly ConcurrentDictionary<string, BlobServiceClient> BlobServiceClientsCache = new ConcurrentDictionary<string, BlobServiceClient>();
    
    public BlobServiceClient GetBlobServiceClient(string connection)
    {
        return BlobServiceClientsCache.GetOrAdd(connection, conn => new BlobServiceClient(conn));
    }
}