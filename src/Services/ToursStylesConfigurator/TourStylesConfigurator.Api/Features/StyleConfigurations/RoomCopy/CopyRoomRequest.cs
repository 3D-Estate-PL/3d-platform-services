using TourStylesConfigurator.Api.Features.StyleConfigurations.Model;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.RoomCopy;

public class CopyRoomRequest
{
    public string ConfigurationId { get; set; }
    public string ConfigurationStyleId { get; set; }
    public string RoomId { get; set; }

    public string? CustomName { get; set; }
    public PlaceType Place { get; set; }

}

public class CopyRoomResponse
{
    public string ConfigurationId { get; set; }
    public string ConfigurationStyleId { get; set; }

    public RoomResponseDto Room { get; set; }
}

public class RoomResponseDto
{
    public string Id { get; set; }
    public string CustomName { get; set; }
    public string RoomType { get; set; }
    
    public RoomStyleBaseDto Style { get; set; }
}