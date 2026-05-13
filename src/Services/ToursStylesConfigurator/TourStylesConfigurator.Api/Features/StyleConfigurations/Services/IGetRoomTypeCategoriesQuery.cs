using System.Dynamic;
using BuildingBlocks.Abstractions.Excel;
using TourStylesConfigurator.Api.Features.Dictionaries.GetCategories;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.Services;

public interface IGetRoomTypeCategoriesQuery
{
    public Task<List<RoomCategoryDto>> GetRoomTypeCategories(string language);
}

public class GetRoomTypeCategoriesQuery : IGetRoomTypeCategoriesQuery
{
    private readonly IExcelDictionaryProvider _excelDictionaryProvider;
    private readonly IGetProductCategoriesQuery _getProductCategoriesQuery;
    private readonly ConfigurationsSettings _configurationsSettings;

    public GetRoomTypeCategoriesQuery(IExcelDictionaryProvider excelDictionaryProvider, IGetProductCategoriesQuery getProductCategoriesQuery, ConfigurationsSettings configurationsSettings)
    {
        _excelDictionaryProvider = excelDictionaryProvider;
        _getProductCategoriesQuery = getProductCategoriesQuery;
        _configurationsSettings = configurationsSettings;
    }

    public  async Task<List<RoomCategoryDto>> GetRoomTypeCategories(string language)
    {
        var productCategories = await _getProductCategoriesQuery.GetAll(language);
        
        var roomTypeCategories = await GetRoomTypesCategories(productCategories);

        return roomTypeCategories.Distinct().ToList();
    }
    
    private async Task<List<RoomCategoryDto>> GetRoomTypesCategories(List<CategoryDto> productCategories)
    {
        var configurationItems = await _excelDictionaryProvider.Import(
            "RoomTypesCategories", new SheetParameters
            {
                RangeRowStart = 1,
                RangeRowEnd = 200,
                RangeColumnStart = 1,
                RangeColumnEnd = 2,
                SpreadSheetId = _configurationsSettings.ConfigurationsSheetId
            });

        var roomTypesCategories = new List<RoomCategoryDto>();

        foreach (dynamic configurationItem in configurationItems)
        {
            if (GetValue(configurationItem, "RoomType") != null &&
                GetValue(configurationItem, "Category") != null)
            {
                roomTypesCategories.Add(new RoomCategoryDto
                {
                    RoomType = configurationItem.RoomType,
                    Category = new CategoryDto
                    {
                        Name = configurationItem.Category,
                        DisplayName = productCategories.FirstOrDefault(x=>x.Name.ToUpper() == configurationItem.Category.ToUpper() 
                        )?.DisplayName
                    }
                });
            }
        }

        return roomTypesCategories;
    }
    
    private dynamic? GetValue(ExpandoObject eo, string key)
    {
        return eo.Where(v => v.Key == key).Select(x => x.Value).FirstOrDefault();
    }
    
}