using System.Dynamic;
using BuildingBlocks.Abstractions.Excel;
using Microsoft.Extensions.Caching.Memory;
using TourStylesConfigurator.Api.Infrastructure.Storage;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.Services;

public interface IGetTourStylesQuery
{
    public Task<List<TourStyleDto>> GetTourStyles(string roomType);
}

public class GetTourStylesQuery : IGetTourStylesQuery
{
    private readonly IExcelDictionaryProvider _excelDictionaryProvider;
    private readonly IMemoryCache _cache;
    private readonly ImageStorageSettings _imageStorageSettings;
    private readonly ConfigurationsSettings _configurationsSettings;

    public GetTourStylesQuery(IExcelDictionaryProvider excelDictionaryProvider, 
        IMemoryCache cache, ImageStorageSettings imageStorageSettings, ConfigurationsSettings configurationsSettings)
    {
        _excelDictionaryProvider = excelDictionaryProvider;
        _cache = cache;
        _imageStorageSettings = imageStorageSettings;
        _configurationsSettings = configurationsSettings;
    }

    public async Task<List<TourStyleDto>> GetTourStyles(string roomType)
    {

        List<TourStyleDto> tourStyles = null;
        var key = $"tourstyles_{roomType}";
        if (_cache.TryGetValue(key, out tourStyles))
        {
            return tourStyles.ToList();
        }
        
        var configurationItems = await _excelDictionaryProvider.Import(
            "Styles", new SheetParameters
            {
                RangeRowStart = 1,
                RangeRowEnd = 300,
                RangeColumnStart = 1,
                RangeColumnEnd = 7,
                SpreadSheetId = _configurationsSettings.ConfigurationsSheetId
            });

        tourStyles = new List<TourStyleDto>();
        foreach (dynamic configurationItem in configurationItems)
            if (GetValue(configurationItem, "Kind") != null &&
                GetValue(configurationItem, "Group") != null)
            {
                var tourStyle = new TourStyle
                {
                    Group = configurationItem.Group,
                    Kind = configurationItem.Kind
                };
                
                var tourStyleDto = new TourStyleDto
                {
                    TourStyle = tourStyle,
                    DisplayName = configurationItem.DisplayName_PL,
                    Description = configurationItem.Description,
                    ThumbnailUrl = StylesHelper.GetStyleThumbnailByConventionUrl(tourStyle, roomType,_imageStorageSettings),
                    RoundThumbnailUrl =  GetValue(configurationItem, "ThumbnailURL")
                };

                var styleConfigurator = (bool)bool.Parse(configurationItem.StyleConfigurator);
                var platform = (bool)bool.Parse(configurationItem.Platform);
                
                if (styleConfigurator)
                {
                    tourStyleDto.Contexts.Add(StyleContext.Configurator);
                }
                if (platform)
                {
                    tourStyleDto.Contexts.Add(StyleContext.Platform);
                }
                

                var additionalRooms = new string []{ "Kitchen","MasterBedroom","Kidsroom"};

                foreach (var additionalRoomType in additionalRooms)
                {
                    var imageUrl = StylesHelper.GetGenericImageUrl(tourStyle, additionalRoomType,_imageStorageSettings);
                   tourStyleDto.Images.Add(imageUrl);
                }
                
                tourStyles.Add(tourStyleDto);
            }
              

        _cache.Set(key, tourStyles,TimeSpan.FromMinutes(5));
        return tourStyles.ToList();
    }

    private dynamic? GetValue(ExpandoObject eo, string key)
    {
        return eo.Where(v => v.Key == key).Select(x => x.Value).FirstOrDefault();
    }
}