using TourStylesConfigurator.Api.Features.InteriorsDesigners.Products.GetProductDetails;
using TourStylesConfigurator.Api.Features.StyleConfigurations.RoomGetProducts;
using TourStylesConfigurator.Api.Features.StyleConfigurations.Services;
using TourStylesConfigurator.Api.Infrastructure.Storage;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Styles;
using CategoryDto = TourStylesConfigurator.Api.Features.Dictionaries.GetCategories.CategoryDto;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.RoomGetResetOptions;

public class Endpoint : Endpoint<GetRoomProductResetOptionsRequest, GetRoomProductResetOptionsResponse>
{
    private readonly ITourStyleConfigurationRepository _tourStyleConfigurationRepository;
    private readonly IGetInteriorDesignerProducts _getInteriorDesignerProducts;
    private readonly IGetTourStylesQuery _getTourStylesQuery;
    private readonly IGetProductCategoriesQuery _getProductCategoriesQuery;
    private IStyleDefaultProductsProvider _styleDefaultProductsProvider;
    private readonly ImageStorageSettings _imageStorageSettings;

    public Endpoint(ITourStyleConfigurationRepository tourStyleConfigurationRepository, IGetInteriorDesignerProducts getInteriorDesignerProducts, IGetTourStylesQuery getTourStylesQuery, IGetProductCategoriesQuery getProductCategoriesQuery, IStyleDefaultProductsProvider styleDefaultProductsProvider, ImageStorageSettings imageStorageSettings)
    {
        _tourStyleConfigurationRepository = tourStyleConfigurationRepository;
        _getInteriorDesignerProducts = getInteriorDesignerProducts;
        _getTourStylesQuery = getTourStylesQuery;
        _getProductCategoriesQuery = getProductCategoriesQuery;
        _styleDefaultProductsProvider = styleDefaultProductsProvider;
        _imageStorageSettings = imageStorageSettings;
    }


    public override void Configure()
    {
        Get($"/{Paths.TourStyleConfigurations}/{{ConfigurationId}}/{Paths.Place}/{Paths.Styles}/{{ConfigurationStyleId}}/{Paths.Rooms}/{{RoomId}}/{Paths.Products}/{{CategoryName}}");
        AllowAnonymous();
        Description(b => b
                .Produces<GetRoomProductResetOptionsResponse>(200, "application/json"),
            clearDefaults: true);
        Summary(s => {
            s.ExampleRequest = new GetRoomProductResetOptionsRequest()
            {
                ConfigurationId = "61e556cf-eb0a-4aa5-8d9d-02937e93e9c9",
                ConfigurationStyleId = "5e1b3427-9a1e-4cb1-8f5e-ef97ffa699ab",
                RoomId = "RoomId",
                CategoryName = "CategoryName"
            };
            s.Summary = "Get Room Products Reset Options";
            s.Description =  "Get Room Products Reset Options";
            s.Responses[200] = "Empty";
        });
    }

    public override async Task HandleAsync(GetRoomProductResetOptionsRequest productResetOptionsRequest, CancellationToken c)
    {
        var userLangs = HttpContext.Request.Headers["accept-language"].ToString();
        var language = userLangs.Split(',').FirstOrDefault();
        
        var configuration = await _tourStyleConfigurationRepository.GetConfigurationAsync(productResetOptionsRequest.ConfigurationId);

        var configurationStyle =
            configuration.GetPlace(productResetOptionsRequest.Place).Styles.Single(x => x.Id == productResetOptionsRequest.ConfigurationStyleId);

        var room = configurationStyle.RoomsConfigurations.Single(x => x.Id == productResetOptionsRequest.RoomId);

        var roomProduct = room.GetProducts().SingleOrDefault(x => x.CategoryName.ToLower() == productResetOptionsRequest.CategoryName.ToLower());

        if (roomProduct == null)
        {
            ThrowError("Product Not Found");
        }

        var defaultProductsForRoom = await _styleDefaultProductsProvider.GetDefaultProductsForRoom(room.Type, room.SelectedStyle.BaseStyle.ToString());
        var commonStyleProduct = configurationStyle.GetCommonRoomProduct(room.Type, productResetOptionsRequest.CategoryName);

        var roomStyleProductForCategory = defaultProductsForRoom.SingleOrDefault(x =>
            x.CategoryName.ToLower() == productResetOptionsRequest.CategoryName.ToLower());

        if (commonStyleProduct?.ProductId == null)
        {
            ThrowError("Common product not found.");
        }
        
        if (roomStyleProductForCategory?.ProductId == null)
        {
            ThrowError("Common product not found.");
        }
        
        var products = (await _getInteriorDesignerProducts.GetProducts(new List<string>
        {
            commonStyleProduct.ProductId, 
            roomStyleProductForCategory.ProductId
        }.ToList(), configuration.InteriorDesignerCode, HttpContext.GetLanguageFromHeader()));

        var commonStyleProductDetails = products.SingleOrDefault(x=>x.Id ==  commonStyleProduct?.ProductId);
        if (commonStyleProductDetails == null)
        {
            ThrowError("CommonProductDetails product not found.");
        }
        
        var commonStyleProductDto = await ConvertToDto(ConvertToDto(commonStyleProduct), language, 
            commonStyleProductDetails);
        
        var roomProductDetails = products.SingleOrDefault(x=>x.Id ==  roomStyleProductForCategory?.ProductId);
        if (roomProductDetails == null)
        {
            ThrowError("RoomProductDetails product not found.");
        }
        
        var roomStyleProductForCategoryDto = await ConvertToDto(roomStyleProductForCategory, language, 
            roomProductDetails);
        

        await SendAsync(new GetRoomProductResetOptionsResponse()
        {
            RoomId = room.Id,
            ProductId = roomProduct.ProductId,
            CommonStyle = commonStyleProductDto,
            RoomStyle = roomStyleProductForCategoryDto,

        }, cancellation: c);
    }

    private  ProductCategoryDto? ConvertToDto(ProductItem? productItem)
    {
        if (productItem == null)
        {
            ThrowError("Common Product not found.");
        }
        
        return new ProductCategoryDto
        {
            ProductId = productItem.ProductId,
            CategoryName = productItem.CategoryName,
            ProductSource = productItem.ProductSource
        };
    }
    
    private async Task<RoomProductItemDto> ConvertToDto(ProductCategoryDto? productItem,
        string language, InteriorDesignerProductDto product)
    {
        if (productItem == null)
        {
            ThrowError("Room Product not found.");
        }
        
        var roomProductItemDto = new RoomProductItemDto
        {
            ProductId = productItem.ProductId,
            ProductSource = productItem.ProductSource,
            Category = new CategoryDto
            {
                Name = productItem.CategoryName
            },
        };

        var productDetails = product;
        if (productDetails != null)
        {
            var productCategory = await _getProductCategoriesQuery.Get(roomProductItemDto.Category.Name, language);
            roomProductItemDto.Category.DisplayName = productCategory?.DisplayName;
            roomProductItemDto.Description = productDetails.Description;
            roomProductItemDto.Price = productDetails?.Price;
            roomProductItemDto.DisplayName = productDetails.DisplayName;
            roomProductItemDto.ThumbnailUrl =
                ThumbnailUrlHelper.GetUrl(productItem.ProductId, _imageStorageSettings);
        }

        return roomProductItemDto;
    }
}