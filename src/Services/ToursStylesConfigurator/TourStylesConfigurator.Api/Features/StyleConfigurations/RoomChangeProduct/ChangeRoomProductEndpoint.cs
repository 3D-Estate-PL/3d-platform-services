using TourStylesConfigurator.Api.Features.Dictionaries.GetCategories;
using TourStylesConfigurator.Api.Features.StyleConfigurations.RoomGetProducts;
using TourStylesConfigurator.Api.Features.StyleConfigurations.Services;
using TourStylesConfigurator.Api.Infrastructure.Storage;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Styles;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.RoomChangeProduct;

public class Endpoint : Endpoint<ChangeRoomProductRequest, ChangeRoomProductResponse>
{
    private readonly ITourStyleConfigurationRepository _tourStyleConfigurationRepository;
    private readonly IGetInteriorDesignerProducts _getInteriorDesignerProducts;
    private IStyleDefaultProductsProvider _styleDefaultProductsProvider;
    private readonly ImageStorageSettings _imageStorageSettings;


    public Endpoint(ITourStyleConfigurationRepository tourStyleConfigurationRepository, IGetInteriorDesignerProducts getInteriorDesignerProducts, IStyleDefaultProductsProvider styleDefaultProductsProvider, ImageStorageSettings imageStorageSettings)
    {
        _tourStyleConfigurationRepository = tourStyleConfigurationRepository;
        _getInteriorDesignerProducts = getInteriorDesignerProducts;
        _styleDefaultProductsProvider = styleDefaultProductsProvider;
        _imageStorageSettings = imageStorageSettings;
    }

    public override void Configure()
    {
        Put($"/{Paths.TourStyleConfigurations}/{{ConfigurationId}}/{Paths.Place}/{Paths.Styles}/{{ConfigurationStyleId}}/{Paths.Rooms}/{{RoomId}}/{Paths.Products}/{{CategoryName}}");
        AllowAnonymous();
        Description(b => b
                .Accepts<ChangeRoomProductRequest>("application/json")
                .Produces<ChangeRoomProductResponse>(200, "application/json"),
            true);
        Summary(s =>
        {
            s.Summary = "Change Product In Selected Category";
            s.Description ="Change Product In Selected Category";
            s.ExampleRequest = new ChangeRoomProductRequest
            {
                ConfigurationId = "61e556cf-eb0a-4aa5-8d9d-02937e93e9c9",
                ConfigurationStyleId = "5e1b3427-9a1e-4cb1-8f5e-ef97ffa699ab",
                RoomId = "roomID",
                CategoryName = "Desk"
            };
            s.Responses[200] = "Empty";
        });
    }

    public override async Task HandleAsync(ChangeRoomProductRequest request, CancellationToken c)
    {
        var configuration = await _tourStyleConfigurationRepository.GetConfigurationAsync(request.ConfigurationId);

        if (configuration == null)
        {
            ThrowError("Configuration Not Found.");
        }
        
        if (configuration.Status != StyleConfigurationStatus.Draft)
        {
            ThrowError("Can not change style. Configuration is marked as readonly.");
        }
        
        var styleConfiguration = configuration.GetPlace(request.Place).Styles.Single(x => x.Id == request.ConfigurationStyleId);


        var roomItem = styleConfiguration.RoomsConfigurations.SingleOrDefault(x => x.Id == request.RoomId);

        if (roomItem == null)
        {
            ThrowError("Room not found");
        }

        var defaultProducts = await _styleDefaultProductsProvider.GetDefaultProductsForRoom(roomItem.Type,
            roomItem.SelectedStyle.BaseStyle.ToString());
        
        
        roomItem.ChangeProduct(request.CategoryName, request.DestinationProductId, defaultProducts);
        var product = roomItem.GetProducts().Single(x => x.CategoryName.ToLower() == request.CategoryName.ToLower());

        await _tourStyleConfigurationRepository.UpsertConfigurationAsync(configuration);
        var productsDetails = (await _getInteriorDesignerProducts.GetProducts(
            new List<string>{request.DestinationProductId}, 
            configuration.InteriorDesignerCode,HttpContext.GetLanguageFromHeader())).FirstOrDefault();
        
        await SendAsync(new ChangeRoomProductResponse
        {
           Room = new RoomProductItemDto
           {
                ProductId = request.DestinationProductId,
                Description = productsDetails?.Description,
                Price = productsDetails?.Price,
                IsOverridden = product.IsOverridden,
                ProductSource = product.ProductSource,
                Category = new CategoryDto
                {
                    Name = product.CategoryName,
                    DisplayName = productsDetails?.CoreProductData.Category.DisplayName
                },
                DisplayName= productsDetails?.DisplayName,
                ThumbnailUrl = 
                    ThumbnailUrlHelper.GetUrl(product.ProductId,_imageStorageSettings)
           }
        }, cancellation: c);
    }
}