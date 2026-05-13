using Azure.Storage.Blobs;
using TourStylesConfigurator.Api.Features.StyleConfigurations.Services;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;

namespace ToursConfigurator.Api.Features.Validation;

public class ValidateResponse
{
    public List<string> Errors { get; set; } = new List<string>();
}

public class ValidateImagesEndpoint : Endpoint<EmptyRequest,ValidateResponse>
{
    private readonly IGetTourStylesQuery _getTourStylesQuery;
    private readonly IGetRoomTypesQuery _getRoomTypesQuery;

    public ValidateImagesEndpoint(IGetTourStylesQuery getTourStylesQuery, IGetRoomTypesQuery getRoomTypesQuery)
    {
        _getTourStylesQuery = getTourStylesQuery;
        _getRoomTypesQuery = getRoomTypesQuery;
    }


    public override void Configure()
    {
        Get("/images/validate");
        AllowAnonymous();
        Description(b => b
                .Accepts<EmptyRequest>("application/json")
                .Produces<EmptyResponse>(200, "application/json"),
            true);

    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var roomTypes= await _getRoomTypesQuery.GetTourConfigurationRoomTypes("EN");


        var errors = new List<string>();
        foreach (var roomType in roomTypes)
        {
            var styles = await _getTourStylesQuery.GetTourStyles(roomType.RoomType);
            foreach (var style in styles)
            {
                var imageBlobUrl = GetGenericImageUrl(style.TourStyle, roomType.RoomType);
                var blobUri = new Uri(imageBlobUrl);
                var blobClient = new BlobClient(blobUri);
                var exists = await blobClient.ExistsAsync();

                if (exists == false)
                {
                    errors.Add($"File Not Exist: {imageBlobUrl}");
                }

            }
        }
        
        
        await SendAsync(new ValidateResponse
        {
            Errors = errors
        }, cancellation: ct);
    }
    
    public static string GetGenericImageUrl(TourStyle style, string roomTypeName)
    {
        var url = $"https://3dstgtourproducts.blob.core.windows.net/styles-images/basic.{style.Kind.ToLower()}.{roomTypeName.ToLower()}.png";
        return url;
    }
}