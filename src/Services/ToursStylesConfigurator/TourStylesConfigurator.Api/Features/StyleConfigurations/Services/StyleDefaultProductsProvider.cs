using TourStylesConfigurator.Api.Features.Dictionaries.GetCategories;
using TourStylesConfigurator.Api.Infrastructure.InteriorsDesigners.Products;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Styles;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.Services;

public class StyleDefaultProductsProvider : IStyleDefaultProductsProvider
{
    private readonly ICoreProductsServiceClient _coreProductsServiceClient;
    private readonly IGetRoomTypeCategoriesQuery _getRoomTypeCategoriesQuery;

    public StyleDefaultProductsProvider(ICoreProductsServiceClient coreProductsServiceClient, IGetRoomTypeCategoriesQuery getRoomTypeCategoriesQuery)
    {
        _coreProductsServiceClient = coreProductsServiceClient;
        _getRoomTypeCategoriesQuery = getRoomTypeCategoriesQuery;
    }

    public async Task<List<DefaultProductsDto>> GetDefaultProducts(string styleId)
    {
        
            var roomTypesCategories = await _getRoomTypeCategoriesQuery.GetRoomTypeCategories("EN");
            return await GetDefaultProductsImpl(styleId, roomTypesCategories);
    }

    private async Task<List<DefaultProductsDto>> GetDefaultProductsImpl(string styleId,
        List<RoomCategoryDto> roomTypesCategories)
    {

        var result = await _coreProductsServiceClient.GetDefaultStylesProducts(); 
        var defaultStyleProducts =  result.
            SingleOrDefault(x=>x.Code == styleId);
        
        return defaultStyleProducts.RoomTypes.Select(x => new DefaultProductsDto
        {
            RoomType = x.Type,
            DefaultProducts = GetDefaultProductsForRoom(x,roomTypesCategories).ToList()
            
        }).ToList();
    }

    private static IList<ProductCategoryDto> GetDefaultProductsForRoom(Room room, 
        List<RoomCategoryDto> roomTypesCategories)
    {
        var result = new List<ProductCategoryDto>();
        foreach (var roomTypeCategory in roomTypesCategories.Where(x=>x.RoomType == room.Type))
        {
            var item = new ProductCategoryDto
            {
                CategoryName = roomTypeCategory.Category.Name,
                ProductId = room.DefaultProducts.FirstOrDefault(x=>
                    x.CategoryName.ToUpper() == roomTypeCategory.Category.GetNameWithRoomPrefix(room.Type).ToUpper())?.ProductId,
                ProductSource = ProductSource.RoomStyle
            };
            result.Add(item);
        }
        
        return result;
    }

    public async Task<List<ProductCategoryDto>> GetDefaultProductsForRoom(string roomType, string styleId)
    {
        var result = await GetDefaultProducts(styleId);
        return result.Where(x => x.RoomType.ToLower() == roomType.ToLower()).SelectMany(t => t.DefaultProducts).ToList();
    }
}


public class GetDefaultStylesProducts
{
    public List<StyleDefaultProducts> Items { get; set; }
}

