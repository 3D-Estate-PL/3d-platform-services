using TourStylesConfigurator.Api.Features.StyleConfigurations.Model;
using TourStylesConfigurator.Api.Features.StyleConfigurations.RoomGetAvailableStyles;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Styles;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.RoomChangeStyle;

public class ChangeRoomStyleRequest
{
    public string ConfigurationId { get; set; }
    public string ConfigurationStyleId { get; set; }
    public string RoomId { get; set; }

    public PlaceType Place { get; set; }

    public RoomStyleBaseDto Style { get; set; }
}

public class ChangeRoomStyleResponse
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
    public RoomStyleDto Style { get; set; }

    public List<ProductItem> Products { get; set; }
}