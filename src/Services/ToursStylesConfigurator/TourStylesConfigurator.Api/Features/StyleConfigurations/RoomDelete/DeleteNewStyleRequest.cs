using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.RoomDelete;

public class DeleteRoomRequest
{
    public PlaceType Place { get; set; }
    public string ConfigurationId { get; set; }
    public string ConfigurationStyleId { get; set; }
    public string RoomId { get; set; }
}


public class DeleteRoomResponse
{
   
}

