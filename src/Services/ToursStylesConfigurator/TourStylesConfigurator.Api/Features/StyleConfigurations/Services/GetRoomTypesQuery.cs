using System.Dynamic;
using BuildingBlocks.Abstractions.Excel;
using Microsoft.Extensions.Caching.Memory;
using TourStylesConfigurator.Api.Features.Dictionaries.GetAvailableRoomTypes;
using TourStylesConfigurator.Api.Features.Dictionaries.GetTourRoomTypes;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.Services;

public interface IGetRoomTypesQuery
{
    public Task<List<AvailableRoomTypeDto>> GetTourConfigurationRoomTypes(string language = null);
    public Task<List<TourRoomTypeDto>> GetTourRoomTypes(string language);
}

public class GetRoomTypesQuery : IGetRoomTypesQuery
{
    private readonly IExcelDictionaryProvider _excelDictionaryProvider;
    private readonly IMemoryCache _memoryCache;
    private readonly ConfigurationsSettings _configurationsSettings;

    public GetRoomTypesQuery(IExcelDictionaryProvider excelDictionaryProvider, IMemoryCache memoryCache, ConfigurationsSettings configurationsSettings)
    {
        _excelDictionaryProvider = excelDictionaryProvider;
        _memoryCache = memoryCache;
        _configurationsSettings = configurationsSettings;
    }

    public async Task<List<AvailableRoomTypeDto>> GetTourConfigurationRoomTypes(string language = null)
    {
        var key = $"cached_room_types_{language}";
        return await _memoryCache.GetOrCreateAsync(key, entry =>
        {
            entry.SetAbsoluteExpiration(TimeSpan.FromHours(1));
            return GetTourConfigurationRoomTypesImpl(language);
        });
    }

    private async Task<List<AvailableRoomTypeDto>> GetTourConfigurationRoomTypesImpl(string language)
    {
        var configurationItems = await _excelDictionaryProvider.Import(
            "RoomTypes", new SheetParameters
            {
                RangeRowStart = 1,
                RangeRowEnd = 50,
                RangeColumnStart = 1,
                RangeColumnEnd = 5,
                SpreadSheetId = _configurationsSettings.ConfigurationsSheetId
            });

        var roomTypes = new List<AvailableRoomTypeDto>();
        foreach (dynamic configurationItem in configurationItems)
        {
            roomTypes.Add(new AvailableRoomTypeDto
            {
                RoomType = configurationItem.Type,
                DisplayName = GetTranslatedDisplayName(configurationItem, language),
                IsRequired = bool.Parse(configurationItem.IsRequired),
                Category = configurationItem.Category,
            });
        }


        return roomTypes;
    }

    public async Task<List<TourRoomTypeDto>> GetTourRoomTypes(string language)
    {
        var configurationItems = await _excelDictionaryProvider.Import(
            "TourRoomTypes", new SheetParameters
            {
                RangeRowStart = 1,
                RangeRowEnd = 50,
                RangeColumnStart = 1,
                RangeColumnEnd = 3,
                SpreadSheetId = _configurationsSettings.ConfigurationsSheetId
            });

        var roomTypes = new List<TourRoomTypeDto>();
        foreach (dynamic configurationItem in configurationItems)
            if (GetValue(configurationItem, "Type") != null)
                roomTypes.Add(new TourRoomTypeDto
                {
                    RoomType = configurationItem.Type,
                    DisplayName = GetTranslatedDisplayName(configurationItem, language),
                });

        


        return roomTypes;    
        
    }
    
    string GetTranslatedDisplayName(dynamic configurationItem, string language)
    {
        if (string.IsNullOrEmpty(language))
        {
            return configurationItem.DisplayName_EN;
        }
        
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