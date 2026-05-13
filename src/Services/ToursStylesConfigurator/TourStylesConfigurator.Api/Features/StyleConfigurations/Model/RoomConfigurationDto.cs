using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.Model;

public class RoomConfigurationDto
{
    public string Id { get; set; }
    public string RoomType { get; set; }
    public string CustomName { get; set; }
    
    public bool IsRequired { get; set; }
    public RoomStyleBaseDto Style { get; set; }
}


public class RoomStyleBaseDto
{
    public TourStyle BaseStyle { get; set; }
    public bool IsCustom { get; set; }
}