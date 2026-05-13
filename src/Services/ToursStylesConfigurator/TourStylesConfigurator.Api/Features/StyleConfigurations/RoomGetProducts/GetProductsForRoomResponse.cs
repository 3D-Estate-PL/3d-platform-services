using TourStylesConfigurator.Api.Features.Dictionaries.GetCategories;
using TourStylesConfigurator.Api.Features.StyleConfigurations.RoomGetAvailableStyles;
using TourStylesConfigurator.Api.Features.StyleConfigurations.Services;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Styles;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.RoomGetProducts;

public class GetProductsForRoomResponse
{
    public string RoomId { get; set; }

    public RoomStyleDto Style { get; set; }
    public string CustomName { get; set; }
    public List<RoomProductItemDto> Products { get; set; } = new();
}

public class TourStyleDto
{
    public TourStyle BaseStyle { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public string ThumbnailUrl { get; set; }
}

public class RoomProductItemDto
{
    public string ProductId { get; set; }
    
    public string InteriorDesignerCode { get; set; }
    public CategoryDto Category { get; set; }
    
    public SubCategoryDto SubCategory { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public string ThumbnailUrl { get; set; }
    public bool IsOverridden { get; set; }
    
    public decimal? Price { get; set; }
    public ProductSource ProductSource { get; set; }
}