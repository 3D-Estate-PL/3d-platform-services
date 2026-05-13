namespace TourStylesConfigurator.Api.Features.Dictionaries.GetAvailableRoomTypes;

public class AvailableRoomTypeDto
{
    public string RoomType { get; set; }
    public string DisplayName { get; set; }
    
    public bool IsRequired { get; set; }
    public string Category { get; set; }
}