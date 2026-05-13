using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using BuildingBlocks.Application.Exceptions.Exceptions;
using Dapr.Client.Autogen.Grpc.v1;
using ImagesManagement.Application.Services;
using ImagesManagement.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace ImagesManagement.Infrastructure;



public class ImageResizer : IImageResizer
{
    private readonly ILogger<ImageResizer> _logger;
    private readonly HttpClient _httpClient;

    public ImageResizer(ILogger<ImageResizer> logger,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task Resize(string container, string sourceFileName, 
        ImageResolutionEntity imageResolutionEntity,
        BlobServiceClient blobServiceClient,
        string? destinationFileName)
    {
        try
        {
            await ResizeImpl(container, sourceFileName, imageResolutionEntity, blobServiceClient, destinationFileName);
            _logger.LogDebug("File Resized. Source :{SourceFileName}. Desitnation {DestinationFileName}", sourceFileName, destinationFileName);
        }
        catch (RequestFailedException e)
        {
            _logger.LogWarning("Condition not met. Source:{SourceFileName}. Desitnation {DestinationFileName}", sourceFileName, destinationFileName);
            if (e.ErrorCode != BlobErrorCode.ConditionNotMet && 
                e.ErrorCode != BlobErrorCode.BlobNotFound)
            {
                throw;
            }
        }
    }

    private async Task ResizeImpl(string container, string sourceFileName, ImageResolutionEntity imageResolutionEntity,
        BlobServiceClient blobServiceClient, string? destinationFileName)
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(container);
        var blobSourceClient = blobContainerClient.GetBlobClient(sourceFileName);
        var blobName = blobSourceClient.Name;

        var absoluteFileName = blobName.Replace($"{Path.GetExtension(blobName)}","");
        
        var fileQualitySuffix = $"{imageResolutionEntity.ImageSuffix()}";

        if (destinationFileName == null)
        {
            destinationFileName = $"{absoluteFileName}__{fileQualitySuffix}{Path.GetExtension(blobName)}";
        }
        else
        {
            destinationFileName = $"{absoluteFileName.Replace(Path.GetFileName(absoluteFileName), destinationFileName)}";
        }

        var blobClient =
            blobContainerClient.GetBlobClient(destinationFileName);
        var blobRequestConditions = await GetBlobRequestConditions(blobClient);
        
        var blobOpenWriteOptions = imageResolutionEntity.Encoder == Encoder.JPG ? new BlobOpenWriteOptions
        {
            OpenConditions = blobRequestConditions,
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = "image/jpeg"
            }
        }: new BlobOpenWriteOptions()
        {
            OpenConditions = blobRequestConditions,
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = "image/png"
            }
        };
        
        await using var inputBlob = await blobSourceClient.OpenReadAsync();
        var image = await Image.LoadAsync(inputBlob);

        IImageEncoder encoder = null;
        if (imageResolutionEntity.Encoder == Encoder.JPG)
        {
            encoder = new JpegEncoder()
            {
                Quality = imageResolutionEntity.Compression
            };
        }
        if (imageResolutionEntity.Encoder == Encoder.PNG)
        {
            encoder = new PngEncoder
            {
                CompressionLevel = MapToPngCompression(imageResolutionEntity.Compression)
            };
        }

        ResizeOptions options = null;
        if (imageResolutionEntity.MaxRes.HasValue)
        {
            if (image.Width > image.Height)
            {
                options = new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size() {Width = imageResolutionEntity.MaxRes.Value}
                };
            }
            else
            {
                options = new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size() {Height = imageResolutionEntity.MaxRes.Value}
                };
            }
          
        }

        if (imageResolutionEntity.Height.HasValue && imageResolutionEntity.Width.HasValue)
        {
            options = new ResizeOptions
            {
                Mode = ResizeMode.Pad,
                Size = new Size {Height = imageResolutionEntity.Height.Value, Width = imageResolutionEntity.Width.Value}
            };
        }
        

        image.Mutate(x =>
        {
            x.Resize(options);
            if (imageResolutionEntity.Encoder == Encoder.JPG)
            {
                x.BackgroundColor(Color.White);
            }
        });

        //Encode here for quality

        if (encoder == null)
        {
            throw new CustomException("Encoder not set.");
        }
       
        await using var output = await blobClient.OpenWriteAsync(true, blobOpenWriteOptions);
        await image.SaveAsync(output, encoder);
        image.Dispose();
    }

    private  async Task<BlobRequestConditions?> GetBlobRequestConditions(BlobClient blobClient)
    {
        if (await blobClient.ExistsAsync() == false)
        {
            return null;
        }
        var properties = await blobClient.GetPropertiesAsync();
        var writeEtag = properties.Value.ETag;
        _logger.LogInformation("ETag:{ETag}. File:{SourceFileName}", 
            writeEtag, blobClient.Name);
        return new BlobRequestConditions{IfMatch = writeEtag};
    }

    private PngCompressionLevel MapToPngCompression(int compression)
    {
        if (compression > 0 && compression <= 10)
        {
            return PngCompressionLevel.Level9;
        }
        if (compression > 10 && compression <= 20)
        {
            return PngCompressionLevel.Level8;
        }
        if (compression > 20 && compression <= 30)
        {
            return PngCompressionLevel.Level7;
        }
        if (compression > 30 && compression <= 40)
        {
            return PngCompressionLevel.Level6;
        }
        if (compression > 40 && compression <= 50)
        {
            return PngCompressionLevel.Level5;
        }
        if (compression > 50 && compression <= 60)
        {
            return PngCompressionLevel.Level4;
        }
        if (compression > 60 && compression <= 70)
        {
            return PngCompressionLevel.Level3;
        }
        if (compression > 70 && compression <= 80)
        {
            return PngCompressionLevel.Level2;
        }
        if (compression > 80 && compression <= 90)
        {
            return PngCompressionLevel.Level1;
        }
        else
        {
            return PngCompressionLevel.Level0;
        }
    }
}