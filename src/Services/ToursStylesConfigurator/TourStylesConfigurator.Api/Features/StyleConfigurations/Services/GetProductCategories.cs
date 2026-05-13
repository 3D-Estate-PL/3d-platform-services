using System.Dynamic;
using BuildingBlocks.Abstractions.Excel;
using Microsoft.Extensions.Caching.Memory;
using TourStylesConfigurator.Api.Features.Dictionaries.GetCategories;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.Services;

public interface IGetProductCategoriesQuery
{
    Task<List<CategoryDto>> GetAll(string language);
    Task<CategoryDto?> Get(string categoryName, string language);
}

public class GetProductCategoriesQuery : IGetProductCategoriesQuery
{
    private readonly IExcelDictionaryProvider _excelDictionaryProvider;
    private readonly IMemoryCache _cache;
    private readonly ConfigurationsSettings _configurationsSettings;

    public GetProductCategoriesQuery(IExcelDictionaryProvider excelDictionaryProvider, IMemoryCache cache, ConfigurationsSettings configurationsSettings)
    {
        _excelDictionaryProvider = excelDictionaryProvider;
        _cache = cache;
        _configurationsSettings = configurationsSettings;
    }

    public async Task<List<CategoryDto>> GetAll(string language)
    {
        List<CategoryDto> productCategories;

        string key = $"categories_{language}";
        if (_cache.TryGetValue(key, out productCategories))
        {
            return productCategories;
        }

        productCategories = new List<CategoryDto>();
        var configurationItems = await _excelDictionaryProvider.Import(
            "Categories", new SheetParameters
            {
                RangeRowStart = 1,
                RangeRowEnd = 200,
                RangeColumnStart = 1,
                RangeColumnEnd = 3,
                SpreadSheetId = _configurationsSettings.ConfigurationsSheetId
            });


        foreach (dynamic configurationItem in configurationItems)
            if (GetValue(configurationItem, "Name") != null)
                productCategories.Add(new CategoryDto
                {
                    Name = configurationItem.Name,
                    DisplayName = GetTranslatedDisplayName(configurationItem, language)
                });

        _cache.Set(key, productCategories, TimeSpan.FromHours(1));

        return productCategories;
    }

    public async Task<CategoryDto?> Get(string categoryName, string language)
    {
        var categories = await GetAll(language);
        return categories.FirstOrDefault(x => x.Name == categoryName);
    }

    private dynamic? GetValue(ExpandoObject eo, string key)
    {
        return eo.Where(v => v.Key == key).Select(x => x.Value).FirstOrDefault();
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
}