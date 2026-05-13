using TourStylesConfigurator.Api.Features.StyleConfigurations.Model;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.GetConfigurationStylesForPlace;

public class GetConfigurationStylesRequest
{
    public string ConfigurationId { get; set; }
    public PlaceType Place { get; set; }
}

public class ConfigurationStyleItemDto 
{
    public ConfigurationStyleItemDto()
    {
        RoomsConfigurations = new List<RoomConfigurationDto>();
    }

    public string Id { get; set; }
    public string CustomName { get; set; }
    
    public string Code { get; set; }

    public List<RoomConfigurationDto> RoomsConfigurations { get; set; }
}