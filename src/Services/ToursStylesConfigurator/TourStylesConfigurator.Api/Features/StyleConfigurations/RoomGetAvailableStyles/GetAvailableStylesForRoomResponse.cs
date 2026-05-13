using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.RoomGetAvailableStyles;

public class GetAvailableStylesForRoomResponse
{
    public string RoomId { get; set; }
    
    public List<RoomStyleDto> AvailableStyles { get; set; }
}

public class RoomStyleDto
{
    public RoomStyleDto(TourStyle baseStyle, bool isCustom, string? displayName, string? description, string? thumbnailUrl)
    {
        BaseStyle = baseStyle;
        IsCustom = isCustom;
        DisplayName = displayName;
        Description = description;
        ThumbnailUrl = thumbnailUrl;
    }

    public string Id =>  new { BaseStyle.Group, BaseStyle.Kind, IsCustom }.GetHashCode().ToString();

    private string _roomType;
    public TourStyle BaseStyle { get; private set; }
    public bool IsCustom { get; private set; }
    public string DisplayName { get; private set; }
    public string Description { get; private set; }
    public string ThumbnailUrl { get;  }
}



