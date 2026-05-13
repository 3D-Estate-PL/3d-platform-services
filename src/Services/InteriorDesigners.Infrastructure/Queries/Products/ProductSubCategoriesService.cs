using System.Dynamic;
using System.Text.Json;
using BuildingBlocks.Abstractions.Excel;
using Microsoft.Extensions.Caching.Memory;

namespace InteriorDesigners.Infrastructure.Queries.Products;

public class SubCategoryDto
{
    private string _subCategoryName;

    public string Name
    {
        get => _subCategoryName;
        set => _subCategoryName = JsonNamingPolicy.CamelCase.ConvertName(value);
    }


    public string? DisplayName { get; set; }
}

public interface IProductSubCategoriesService
{
    Task<List<SubCategoryDto>> GetAll(string language);
}

public class ProductSubCategoriesService : IProductSubCategoriesService
{
    private readonly IExcelDictionaryProvider _excelDictionaryProvider;
    private readonly IMemoryCache _cache;
    private readonly ConfigurationsSettings _configurationsSettings;

    public ProductSubCategoriesService(IExcelDictionaryProvider excelDictionaryProvider, IMemoryCache cache, ConfigurationsSettings configurationsSettings)
    {
        _excelDictionaryProvider = excelDictionaryProvider;
        _cache = cache;
        _configurationsSettings = configurationsSettings;
    }
    
    public async Task<List<SubCategoryDto>> GetAll(string language)
    {
        List<SubCategoryDto> productSubCategories;

        var key = $"subcategories_{language}";
        if (_cache.TryGetValue(key, out productSubCategories))
        {
            return productSubCategories;
        }

        productSubCategories = new List<SubCategoryDto>();
        var configurationItems = await _excelDictionaryProvider.Import(
            "SubCategories", new SheetParameters
            {
                RangeRowStart = 1,
                RangeRowEnd = 200,
                RangeColumnStart = 1,
                RangeColumnEnd = 3,
                SpreadSheetId = _configurationsSettings.ConfigurationsSheetId
            });


        foreach (dynamic configurationItem in configurationItems)
            if (GetValue(configurationItem, "Name") != null)
                productSubCategories.Add(new SubCategoryDto
                {
                    Name = configurationItem.Name,
                    DisplayName = GetTranslatedDisplayName(configurationItem, language)
                });

        _cache.Set(key, productSubCategories, TimeSpan.FromHours(1));

        return productSubCategories;
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