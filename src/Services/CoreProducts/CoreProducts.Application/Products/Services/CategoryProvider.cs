using System.Dynamic;
using BuildingBlocks.Abstractions.Excel;
using CoreProducts.Application.Products.Dtos;

namespace CoreProducts.Application.Products.Services;

public interface ICategoryProvider
{
    public Task<string?>  GetDisplayNameForCategoryType(string categoryType);
}

public class CategoryProvider : ICategoryProvider
{
    private readonly IExcelDictionaryProvider _excelDictionaryProvider;
    private List<CategoryDto>? _categories;
    private readonly ConfigurationsSettings _configurationsSettings;

    public CategoryProvider(IExcelDictionaryProvider excelDictionaryProvider, ConfigurationsSettings configurationsSettings)
    {
        _excelDictionaryProvider = excelDictionaryProvider;
        _configurationsSettings = configurationsSettings;
    }


    public async Task<string?> GetDisplayNameForCategoryType(string categoryType)
    {
        if (_categories == null)
        {
            _categories = await GetCategories();
        }
        
        var category =  _categories.FirstOrDefault(x => x.Name == categoryType);

        if (category == null)
        {
            return "Other";
        }

        return category.DisplayName;
    }
    
    
    
    private async Task<List<CategoryDto>> GetCategories()
    {
        var configurationItems = await _excelDictionaryProvider.Import(
            "Categories", new SheetParameters
            {
                RangeRowStart = 1,
                RangeRowEnd = 200,
                RangeColumnStart = 1,
                RangeColumnEnd = 2,
                SpreadSheetId = _configurationsSettings.ConfigurationsSheetId
            });

        var categories = new List<CategoryDto>();

        foreach (dynamic configurationItem in configurationItems)
        {
            if (GetValue(configurationItem, "Name") != null &&
                GetValue(configurationItem, "DisplayName_PL") != null)
            {
                categories.Add(new CategoryDto
                {
                    Name = configurationItem.Name,
                    DisplayName = configurationItem.DisplayName_PL,
                });
            }
        }

        return categories;
    }

    private dynamic? GetValue(ExpandoObject eo, string key)
    {
        return eo.Where(v => v.Key == key).Select(x => x.Value).FirstOrDefault();
    }
}