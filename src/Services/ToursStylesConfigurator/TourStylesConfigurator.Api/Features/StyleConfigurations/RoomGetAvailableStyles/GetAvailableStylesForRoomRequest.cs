using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.RoomGetAvailableStyles;

public class GetAvailableStylesForRoomRequest
{
    public string ConfigurationId { get; set; }
    public PlaceType Place { get; set; }
    public string ConfigurationStyleId { get; set; }
    public string RoomId { get; set; }
}

