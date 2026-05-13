using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.GetConfigurationStyles;

public class GetConfigurationStylesResponse
{
    public string Code { get; set; }
    public List<RoomItemDto> Rooms { get; set; }
    public TourStyle BaseStyle { get; set; }
    
    public string InteriorDesignerCode { get; set; }

    public GetConfigurationStylesResponse()
    {
        Rooms = new List<RoomItemDto>();
    }
}

public class RoomItemDto
{
    public RoomItemDto()
    {
        Elements = new List<ProductItemDto>();
    }

    public string Room { get; set; }
    
    public int RoomIndex { get; set; }
    public List<ProductItemDto> Elements { get; set; }
}

public class ProductItemDto
{
    public string CategoryName { get; set; }
    public string Id { get; set; }
}