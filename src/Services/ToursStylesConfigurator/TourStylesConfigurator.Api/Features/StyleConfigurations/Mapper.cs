using TourStylesConfigurator.Api.Features.StyleConfigurations.GetConfigurationStylesForPlace;
using TourStylesConfigurator.Api.Features.StyleConfigurations.Model;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Styles;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations;

public static class Mapper
{
    public static ConfigurationStyleItemDto Map(this StyleConfigurationItem style)
    {
        return new ConfigurationStyleItemDto
        {
            Id = style.Id,
            CustomName = style.CustomName,
            Code = style.Code,
            RoomsConfigurations = style.RoomsConfigurations.Where(x=>x.IsDefinedByUser).
                Select(x => new RoomConfigurationDto
            {
                Id = x.Id,
                RoomType = x.Type,
                CustomName = x.CustomName,
                IsRequired = x.IsRequired,
                Style = new RoomStyleBaseDto
                {
                    IsCustom = x.SelectedStyle.IsCustom,
                    BaseStyle = x.SelectedStyle.BaseStyle,
                },
            }).ToList()
        };
    }
}