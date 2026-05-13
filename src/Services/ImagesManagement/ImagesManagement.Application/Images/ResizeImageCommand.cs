using System.Security.Cryptography;
using System.Text;
using System.Web;
using Azure.Storage.Blobs;
using BuildingBlocks.Abstractions.CQRS.CQRS;
using BuildingBlocks.Application.Exceptions.Exceptions;
using ImagesManagement.Application.Services;
using ImagesManagement.Domain;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using Encoder = ImagesManagement.Domain.Encoder;

namespace ImagesManagement.Application.Images;

public class ResizeImageCommandResponse
{
    public ResizeImageCommandResponse(string? fileUrl)
    {
        FileUrl = fileUrl;
    }

    public string? FileUrl { get; }
}


    public class ResizeImageCommand : ICommand<ResizeImageCommandResponse>
    {
        public string StorageKey { get; }
        public string ImageName { get; }
        public int? Width { get; }
        public int? Height { get; }
        public int? MaxRes { get; }
        public Encoder Encoder { get; }
        public int Compression { get; }
        public string? DestinationFileName { get; }
        
        public bool UseCache { get; }

        public ResizeImageCommand(string storageKey, string imageName, int? width, 
            int? height,int? maxRes, Encoder encoder, int compression = 90,
            string? destinationFileName = null, bool useCache = true)
        {
            if (maxRes.HasValue == false && (height.HasValue == false || width.HasValue == false))
            {
                throw new CustomException("Invalid argument. MaxRes or height or width are required.");
            }

            UseCache = useCache;
            StorageKey = storageKey;
            ImageName = imageName;
            Width = width;
            Height = height;
            MaxRes = maxRes;
            Encoder = encoder;
            Compression = compression;
            DestinationFileName = destinationFileName;
        }
    }

    internal class GetImageQueryQueryHandler : ICommandHandler<ResizeImageCommand, ResizeImageCommandResponse>
    {
        private readonly IImageResizer _imageResizer;
        private readonly IImageMetadataRepository _imageMetadataRepository;
        private readonly IImageStorageSettingsProvider _imageStorageSettingsProvider;
        private readonly Tracer _tracer;
        private readonly ILogger<GetImageQueryQueryHandler> _logger;
        private readonly IBlobStorageClientProvider _blobStorageClientProvider;

        public GetImageQueryQueryHandler(IImageResizer imageResizer,
            IImageMetadataRepository imageMetadataRepository, 
            IImageStorageSettingsProvider imageStorageSettingsProvider, Tracer tracer, ILogger<GetImageQueryQueryHandler> logger, IBlobStorageClientProvider blobStorageClientProvider)
        {
            _imageResizer = imageResizer;
            _imageMetadataRepository = imageMetadataRepository;
            _imageStorageSettingsProvider = imageStorageSettingsProvider;
            _tracer = tracer;
            _logger = logger;
            _blobStorageClientProvider = blobStorageClientProvider;
        }

        public async Task<ResizeImageCommandResponse> Handle(ResizeImageCommand request, CancellationToken cancellationToken)
        {
            var originalFileBlobClient = await GetBlobClient(request.ImageName,request.StorageKey);

            var imageBlobUrl = originalFileBlobClient.Uri.ToString(); 
            var containerName = originalFileBlobClient.BlobContainerName;
            var isFileExists = await originalFileBlobClient.ExistsAsync(cancellationToken);

            if (isFileExists == false)
            {
                return new ResizeImageCommandResponse(null);
            }
            
            var blobProperties = await originalFileBlobClient.GetPropertiesAsync();
            var blobLastModifiedDate = blobProperties.Value.LastModified;

            var id = GenerateHashId(request.ImageName);

            var image = request.UseCache ? await _imageMetadataRepository.FindWithPartitionKeyFilter(containerName,id) : null;

            
            if (image == null)
            {
                image = new ImageMetadataAggregate(containerName, id, blobLastModifiedDate);
            }

            var imageResolution =
                new ImageResolutionEntity(request.Height, request.Width, request.MaxRes, request.Compression, request.Encoder);

            if (blobLastModifiedDate != image.LastModified && image.Resolutions.Contains(imageResolution) )
            {
                image.Reset(blobLastModifiedDate);
            }
            
            
            
            if (image.Resolutions.Contains(imageResolution) == false)
            {
                var imageStorageSettings = (await _imageStorageSettingsProvider.GetConfigurationAsync());
                var config = imageStorageSettings
                    .Mapping
                    .Single(x => x.Key == request.StorageKey);

                if (config == null)
                {
                    throw new CustomException("Configuration not found for storage.");
                }
                
                var blobServiceClient = _blobStorageClientProvider.GetBlobServiceClient(config.ConnectionString);
                
                await _imageResizer.Resize(image.Container, originalFileBlobClient.Name, imageResolution, blobServiceClient, request.DestinationFileName);
                image.AddResolution(imageResolution);
            }

            if (request.UseCache)
            {
                await _imageMetadataRepository.UpsertAsync(image);
            }

            var extension = request.ImageName.Split(".").Last();

            return new ResizeImageCommandResponse(GetDestinationImageUrl(request, imageBlobUrl, extension, imageResolution));

            //return new GetImageQueryResponse(imageWithRequestedResolution);
            
            bool DestinationFileNameHasExtension()
            {
                return string.IsNullOrEmpty(Path.GetExtension(request.DestinationFileName)) == false;
            }
        }

        private static string GetDestinationImageUrl(ResizeImageCommand request, string imageBlobUrl,
            string extension, ImageResolutionEntity imageResolution)
        {
            if (request.DestinationFileName == null)
            {
                var imageWithRequestedResolution = $"{imageBlobUrl.Replace($".{extension}","")}__{imageResolution.ImageSuffix()}.{extension}";
                return imageWithRequestedResolution;
            }
            else
            {
                var imageWithRequestedResolution =
                    $"{imageBlobUrl.Replace(Path.GetFileName(imageBlobUrl), request.DestinationFileName)}";

                return imageWithRequestedResolution;
            }
        }


        private async Task<BlobClient> GetBlobClient(string fileName, string storageKey)
        {
            var storageConfiguration = (await _imageStorageSettingsProvider.GetConfigurationAsync()).Mapping.SingleOrDefault(x=>x.Key == storageKey);
            if (storageConfiguration == null)
            {
                throw new CustomException($"Storage configuration not found for key{storageKey}.");
            }
            
            
            var blobUrl = HttpUtility.UrlDecode($"{storageConfiguration.StorageUrl}/{fileName}");
            _logger.LogInformation("BlobUrl:{BlobUrl}", blobUrl);
            
            var originalFileBlobClient = new BlobClient(new Uri(blobUrl));
            var containerClient = _blobStorageClientProvider.GetBlobServiceClient(storageConfiguration.ConnectionString)
                .GetBlobContainerClient(originalFileBlobClient.BlobContainerName);

            if (containerClient == null)
            {
                _logger.LogInformation("BlobContainer is null :{Container}", originalFileBlobClient.BlobContainerName);
                throw new InvalidOperationException("BlobContainer is null");
            }
            
            return containerClient.GetBlobClient(originalFileBlobClient.Name);
        }

        private string GenerateHashId(string data)
        {

            var constData = "AB0734E8-F82D-48BC-9FED-BB5A344288EE";
            byte[] hash;
            using (MD5 md5 = MD5.Create())
            {
                md5.Initialize();
                md5.ComputeHash(Encoding.UTF8.GetBytes($"{data}-{constData}"));
                hash = md5.Hash;
            }

            return BitConverter.ToString(hash).Replace("-","");
        }
        
    }
