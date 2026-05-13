using TourStylesConfigurator.Api.Features.InteriorsDesigners.Products.GetProductDetails;
using TourStylesConfigurator.Api.Features.StyleConfigurations.RoomGetAvailableStyles;
using TourStylesConfigurator.Api.Features.StyleConfigurations.Services;
using TourStylesConfigurator.Api.Infrastructure.Storage;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;
using CategoryDto = TourStylesConfigurator.Api.Features.Dictionaries.GetCategories.CategoryDto;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.RoomGetProducts;

public class Endpoint : Endpoint<GetProductsForRoomRequest, GetProductsForRoomResponse>
{
    private readonly ITourStyleConfigurationRepository _tourStyleConfigurationRepository;
    private readonly IGetInteriorDesignerProducts _getInteriorDesignerProducts;
    private readonly IGetTourStylesQuery _getTourStylesQuery;
    private readonly IGetProductCategoriesQuery _getProductCategoriesQuery;
    private readonly ILogger<Endpoint> _logger;
    private readonly ImageStorageSettings _imageStorageSettings;

    public Endpoint(ITourStyleConfigurationRepository tourStyleConfigurationRepository, IGetInteriorDesignerProducts getInteriorDesignerProducts, IGetTourStylesQuery getTourStylesQuery, IGetProductCategoriesQuery getProductCategoriesQuery, ILogger<Endpoint> logger, ImageStorageSettings imageStorageSettings)
    {
        _tourStyleConfigurationRepository = tourStyleConfigurationRepository;
        _getInteriorDesignerProducts = getInteriorDesignerProducts;
        _getTourStylesQuery = getTourStylesQuery;
        _getProductCategoriesQuery = getProductCategoriesQuery;
        _logger = logger;
        _imageStorageSettings = imageStorageSettings;
    }


    public override void Configure()
    {
        Get($"/{Paths.TourStyleConfigurations}/{{ConfigurationId}}/{Paths.Place}/{Paths.Styles}/{{ConfigurationStyleId}}/{Paths.Rooms}/{{RoomId}}/products");
        AllowAnonymous();
        Description(b => b
                .Produces<GetProductsForRoomResponse>(200, "application/json"),
            clearDefaults: true);
        Summary(s => {
            s.ExampleRequest = new GetProductsForRoomRequest()
            {
                ConfigurationId = "61e556cf-eb0a-4aa5-8d9d-02937e93e9c9",
                ConfigurationStyleId = "5e1b3427-9a1e-4cb1-8f5e-ef97ffa699ab",
                RoomId = "RoomId"
            };
            s.Summary = "Get Products For Room";
            s.Description =  "Get Products For Room";
            s.Responses[200] = "Empty";
        });
    }

    public override async Task HandleAsync(GetProductsForRoomRequest request, CancellationToken c)
    {
        var userLangs = HttpContext.Request.Headers["accept-language"].ToString();
        var language = userLangs.Split(',').FirstOrDefault();
        
        var configuration = await _tourStyleConfigurationRepository.GetConfigurationAsync(request.ConfigurationId);

        var configurationStyle =
            configuration.GetPlace(request.Place).Styles.Single(x => x.Id == request.ConfigurationStyleId);

        var room = configurationStyle.RoomsConfigurations.Single(x => x.Id == request.RoomId);
        var roomProducts = new List<RoomProductItemDto>();

        List<InteriorDesignerProductDto> products = null;
        
        if (room.GetProducts() != null)
        {
            products = await _getInteriorDesignerProducts.GetProducts(room.GetProducts().Select(y => y.ProductId).ToList(),
                configuration.InteriorDesignerCode,HttpContext.GetLanguageFromHeader());
        }
        
        var tourStyles = await _getTourStylesQuery.GetTourStyles(room.Type);
        tourStyles.Add(new StyleConfigurations.TourStyleDto
        {
            TourStyle = new TourStyle
            {
                Group = "Custom",
                Kind = "Custom",
            },
            DisplayName = "Własny",
            Description = "Style własny"
        });

        if (room.GetProducts() != null)
        {
            foreach (var roomProduct in room.GetProducts())
            {
                var roomProductItemDto = new RoomProductItemDto
                {
                    ProductId = roomProduct.ProductId,
                    Category = new CategoryDto
                    {
                        Name = roomProduct.CategoryName
                    },
                    IsOverridden = roomProduct.IsOverridden,
                    ProductSource = roomProduct.ProductSource
                };

                var productDetails = products?.FirstOrDefault(y => y.Id == roomProduct.ProductId);
                if (productDetails != null)
                {
                    var productCategory =
                        await _getProductCategoriesQuery.Get(roomProductItemDto.Category.Name, language);
                    roomProductItemDto.Category.DisplayName = productCategory?.DisplayName;
                    roomProductItemDto.Description = productDetails.Description;
                    roomProductItemDto.Price = productDetails?.Price;
                    roomProductItemDto.DisplayName = productDetails.DisplayName;
                    roomProductItemDto.ThumbnailUrl =
                        ThumbnailUrlHelper.GetUrl(productDetails.Id,_imageStorageSettings);

                    roomProductItemDto.InteriorDesignerCode = productDetails.InteriorDesignerCode;


                    roomProductItemDto.SubCategory = new SubCategoryDto
                    {
                        Name = productDetails.CoreProductData.Category.Name,
                        DisplayName = productDetails.CoreProductData.Category.DisplayName,
                    };
                }
                else
                {
                    _logger.LogWarning("Product with id:{ProductId} not found", roomProduct.ProductId);
                }
                
                roomProducts.Add(roomProductItemDto);
            } 

        }

        var styleDefinition = tourStyles.SingleOrDefault(x => x.TourStyle.Group == room.SelectedStyle.BaseStyle.Group &&
                                                              x.TourStyle.Kind == room.SelectedStyle.BaseStyle.Kind);
        

        await SendAsync(new GetProductsForRoomResponse()
        {
            RoomId = room.Id,
            CustomName = room.CustomName,
            Style =  new RoomStyleDto(room.SelectedStyle.BaseStyle,
                room.SelectedStyle.IsCustom,
                styleDefinition?.DisplayName,
                styleDefinition?.Description, 
                styleDefinition?.ThumbnailUrl),
            Products = roomProducts
        }, cancellation: c);
    }
}