using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.RoomGetProducts;

public class GetProductsForRoomRequest
{
    public string ConfigurationId { get; set; }
    public PlaceType Place { get; set; }
    public string ConfigurationStyleId { get; set; }
    public string RoomId { get; set; }
}

