using System.Dynamic;
using BuildingBlocks.Abstractions.Excel;
using CoreProducts.Application.Products.Dtos;

namespace CoreProducts.Application.Products.Services;

public interface IRoomTypesProvider
{
    public Task<string?> GetDisplayNameForRoomType(string roomType);
}

public class RoomTypesProvider : IRoomTypesProvider
{
    private readonly IExcelDictionaryProvider _excelDictionaryProvider;
    private List<RoomTypeDto>? _roomTypes;
    private readonly ConfigurationsSettings _configurationsSettings;


    public RoomTypesProvider(IExcelDictionaryProvider excelDictionaryProvider, ConfigurationsSettings configurationsSettings)
    {
        _excelDictionaryProvider = excelDictionaryProvider;
        _configurationsSettings = configurationsSettings;
    }


    public async Task<string?> GetDisplayNameForRoomType(string roomType)
    {
        if (_roomTypes == null)
        {
            _roomTypes = await GetRoomTypes();
        }
        var category = _roomTypes.FirstOrDefault(x => x.Type == roomType);

        if (category == null)
        {
            return "Other";
        }

        return category.DisplayName;
    }

    private async Task<List<RoomTypeDto>> GetRoomTypes()
    {
        var configurationItems = await _excelDictionaryProvider.Import(
            "RoomTypes", new SheetParameters
            {
                RangeRowStart = 1,
                RangeRowEnd = 200,
                RangeColumnStart = 1,
                RangeColumnEnd = 3,
                SpreadSheetId = _configurationsSettings.ConfigurationsSheetId
            });

        var roomTypes = new List<RoomTypeDto>();

        foreach (dynamic configurationItem in configurationItems)
            if (GetValue(configurationItem, "Type") != null &&
                GetValue(configurationItem, "DisplayName_PL") != null)
                roomTypes.Add(new RoomTypeDto
                {
                    Type = configurationItem.Type,
                    DisplayName = configurationItem.DisplayName_PL
                });

        return roomTypes;
    }

    private dynamic? GetValue(ExpandoObject eo, string key)
    {
        return eo.Where(v => v.Key == key).Select(x => x.Value).FirstOrDefault();
    }
}