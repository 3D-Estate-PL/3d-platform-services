using TourStylesConfigurator.Api.Features.StyleConfigurations.RoomGetProducts;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.RoomGetResetOptions;

public class GetRoomProductResetOptionsResponse
{
    public string RoomId { get; set; }
    public string ProductId { get; set; }
    public RoomProductItemDto? CommonStyle { get; set; }
    public RoomProductItemDto? RoomStyle { get; set; }
}