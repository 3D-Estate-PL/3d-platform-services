using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.RoomEdit;

public class EditRoomRequest
{
    public string ConfigurationId { get; set; }
    public string ConfigurationStyleId { get; set; }
    public string RoomId { get; set; }

    public PlaceType Place { get; set; }

    public EditRoomRequestDto Room { get; set; }
    public bool AllowOverrideBaseConfiguration { get; set; }
}

public class EditRoomRequestDto
{
    public string CustomName { get; set; }
    public string RoomType { get; set; }
    public TourStyle BaseStyle { get; set; }
}

public class EditRoomResponse
{
    public string ConfigurationId { get; set; }
    public string ConfigurationStyleId { get; set; }

    public EditRoomResponseDto Room { get; set; }
}

public class EditRoomResponseDto
{
    public string Id { get; set; }
    public string CustomName { get; set; }
    public string RoomType { get; set; }
    public TourStyle BaseStyle { get; set; }
}