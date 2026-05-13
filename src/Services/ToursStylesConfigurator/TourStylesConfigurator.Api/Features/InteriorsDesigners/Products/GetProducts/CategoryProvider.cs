using System.Dynamic;
using BuildingBlocks.Abstractions.Excel;
using TourStylesConfigurator.Api.Features.Dictionaries.GetCategories;

namespace TourStylesConfigurator.Api.Features.InteriorsDesigners.Products.GetProducts;

public interface ICategoryProvider
{
    public Task<string?>  GetDisplayNameForCategoryType(string categoryType, string language);
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


    public async Task<string?> GetDisplayNameForCategoryType(string categoryType,string language)
    {
        if (_categories == null)
        {
            _categories = await GetCategories(language);
        }
        
        var category =  _categories.FirstOrDefault(x => string.Equals(x.Name ,categoryType,StringComparison.OrdinalIgnoreCase));

        if (category == null)
        {
            return "Other";
        }

        return category.DisplayName;
    }
    
    
    
    private async Task<List<CategoryDto>> GetCategories(string language)
    {
        var configurationItems = await _excelDictionaryProvider.Import(
            "Categories", new SheetParameters
            {
                RangeRowStart = 1,
                RangeRowEnd = 200,
                RangeColumnStart = 1,
                RangeColumnEnd = 3,
                SpreadSheetId = _configurationsSettings.ConfigurationsSheetId
            });

        var categories = new List<CategoryDto>();

        foreach (dynamic configurationItem in configurationItems)
        {
            if (GetValue(configurationItem, "Name") != null)
            {
                categories.Add(new CategoryDto
                {
                    Name = configurationItem.Name,
                    DisplayName = GetTranslatedDisplayName(configurationItem, language),
                });
            }
        }

        return categories;
    }

    private string GetTranslatedDisplayName(dynamic configurationItem, string language)
    {
        if (string.IsNullOrEmpty(language)) return configurationItem.DisplayName_EN;

        switch (language.ToUpper())
        {
            case "PL":
            {
                return configurationItem.DisplayName_PL;
            }

            default:
            {
                return configurationItem.DisplayName_EN;
            }
        }
    }
    
    private dynamic? GetValue(ExpandoObject eo, string key)
    {
        return eo.Where(v => v.Key == key).Select(x => x.Value).FirstOrDefault();
    }
}