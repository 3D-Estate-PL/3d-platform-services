using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.RoomGetResetOptions;

public class GetRoomProductResetOptionsRequest
{
    public string ConfigurationId { get; set; }
    public PlaceType Place { get; set; }
    public string ConfigurationStyleId { get; set; }
    public string RoomId { get; set; }
    
    public string CategoryName { get; set; }

}

