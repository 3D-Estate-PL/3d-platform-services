using TourStylesConfigurator.Api.Features.Dictionaries.GetAvailableRoomTypes;
using TourStylesConfigurator.Api.Features.StyleConfigurations.Services;

namespace TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Styles;



public class StyleConfigurationItem
{
    public string Id { get; set; }
    public string CustomName { get; set; }
    
    public string Code { get; set; }
    public TourStyle BaseStyle { get; set; }

    private List<RoomItem> _roomItems = new List<RoomItem>();
    public IReadOnlyCollection<RoomItem> RoomsConfigurations => _roomItems;
    
    public Dictionary<string,int> RoomTypeIndex = new Dictionary<string,int>();

    protected StyleConfigurationItem()
    {
        
    }
    
    public static StyleConfigurationItem Exterior()
    {
        return new StyleConfigurationItem
        {
            Id = "default",
            BaseStyle = new TourStyle(),
            CustomName = "default",
            Code = "S01"
        };
    }
    
    public static StyleConfigurationItem Interior(List<AvailableRoomTypeDto> requiredRoomsTypes,
        List<DefaultProductsDto> defaultProductsDtos,
        TourStyle tourStyle, string customName)
    {
        var styleConfigurationItem = new StyleConfigurationItem
        {
            BaseStyle = tourStyle,
            CustomName = customName,
            Id = Guid.NewGuid().ToString()
        };
        
        
        requiredRoomsTypes.
            ForEach(requiredRoomType =>
            {

                var productsForRoom = defaultProductsDtos.Map(requiredRoomType.RoomType);
                var room = RoomItem.New(requiredRoomType.DisplayName, requiredRoomType.RoomType, RoomStyle.New(tourStyle),
                    productsForRoom, isRequired:true);
                styleConfigurationItem.AddRoom(room);
            });

        return styleConfigurationItem;
    }


    public void AddRoom(RoomItem room)
    {
        room.SelectedStyle = RoomStyle.New(BaseStyle);
        _roomItems.Add(room);
        RoomTypeIndex.TryGetValue(room.Type, out var value);
        RoomTypeIndex[room.Type] = ++value;
    }

    public void RemoveRoom(RoomItem room)
    {
        _roomItems.Remove(room);
        RoomTypeIndex[room.Type] -= 1;
    }

    public  StyleConfigurationItem Clone(string code, string? customName = null)
    {
        var result = new StyleConfigurationItem
        {
            Id = Guid.NewGuid().ToString(),
            Code = code,
            BaseStyle = BaseStyle,
            CustomName = customName ?? CustomName,
            _roomItems = _roomItems.Where(x=>x.IsDefinedByUser).ToList().ConvertAll(r=> r.Clone())
        };

        return result;
    }

    public RoomItem GetCommonRoom()
    {
        return RoomsConfigurations.Single(x => x.Type.ToLower() == "common");
    }

    public ProductItem? GetCommonRoomProduct(string roomType, string categoryName)
    {
        if (new List<string> {"bathroom", "common"}.Contains(roomType.ToLower()))
        {
            return null;
        }

        var commonRoom = GetCommonRoom();
        var product = commonRoom.GetProducts().SingleOrDefault(x => x.CategoryName.ToLower() == categoryName.ToLower());
        return product is {ProductSource: ProductSource.CommonStyle} ? product : null;
    }
}