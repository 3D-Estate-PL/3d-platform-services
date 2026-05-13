using System.Text.Json;
using ToursConfigurator.Api.Features.Validation;
using TourStylesConfigurator.Api.Features.StyleConfigurations.Services;

namespace TourStylesConfigurator.Api.Features.Validation;

public class ValidateDefaultProductsRequest
{
    public string? Style { get; set; }
}

public class ValidateDefaultProductsEndpoint : Endpoint<ValidateDefaultProductsRequest,ValidateResponse>
{
    private readonly IGetRoomTypeCategoriesQuery _getRoomTypeCategoriesQuery;
    private readonly ICoreProductsServiceClient _coreProductsService;

    public ValidateDefaultProductsEndpoint(IGetRoomTypeCategoriesQuery getRoomTypeCategoriesQuery, ICoreProductsServiceClient coreProductsService)
    {
        _getRoomTypeCategoriesQuery = getRoomTypeCategoriesQuery;
        _coreProductsService = coreProductsService;
    }


    public override void Configure()
    {
        Get("/styles/defaultproducts/validate");
        AllowAnonymous();
        Description(b => b
                .Accepts<ValidateDefaultProductsRequest>("application/json")
                .Produces<ValidateResponse>(200, "application/json"),
            true);

    }

    public override async Task HandleAsync(ValidateDefaultProductsRequest req, CancellationToken ct)
    {
        var errors = new List<string>();
        var roomTypes = await _getRoomTypeCategoriesQuery.GetRoomTypeCategories("EN");
        var styleDefaultProductsList = await _coreProductsService.GetDefaultStylesProducts();

        if (req.Style != null)
        {
            styleDefaultProductsList = styleDefaultProductsList
                .Where(x => x.Code.ToUpper().Contains(req.Style.ToUpper())).ToList();
        }
        
        foreach (var style in styleDefaultProductsList)
        {

            foreach (var roomType in roomTypes.GroupBy(x => x.RoomType).ToList())
            {
                var roomTypeName = roomType.Key;
                {
                    var styleName = style.Code;
                    var styleDefaultProducts = style.RoomTypes.Single(x => x.Type.ToUpper() == roomTypeName.ToUpper());
                    foreach (var roomTypeCategory in roomType)
                    {
                        if (styleDefaultProducts.DefaultProducts.Any(z =>
                                z.CategoryName.ToUpper() == $"{roomTypeCategory.RoomType.ToUpper()}.{roomTypeCategory.Category.Name.ToUpper()}") == false)
                        {
                            errors.Add($"{styleName} | {roomTypeCategory.RoomType}|  {roomTypeCategory.Category.Name}");    
                        }
                
                    }
                }
            }
        }


        await using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, errors, typeof(List<string>));
        stream.Position = 0;
        await SendStreamAsync(stream, "validationResults.json", stream.Length );
    }
}