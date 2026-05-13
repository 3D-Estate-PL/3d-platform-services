using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.RoomAdd;

public class AddNewRoomRequest
{
    public string ConfigurationId { get; set; }
    
    public PlaceType Place { get; set; }
    public string ConfigurationStyleId { get; set; }

    public List<NewRoomRequestDto> Rooms { get; set; }
}

public class NewRoomRequestDto
{
    public string CustomName { get; set; }
    public string RoomType { get; set; }
}


public class AddNewRoomResponse
{
    public string ConfigurationId { get; set; }
    public string ConfigurationStyleId { get; set; }
    
    public List<NewRoomResponseDto> Rooms { get; set; }
}


public class NewRoomResponseDto
{
    public string Id { get; set; }
    public string CustomName { get; set; }
    public string RoomType { get; set; }
    
    public bool IsRequired { get; set; }
}


