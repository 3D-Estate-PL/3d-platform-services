using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Styles;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.Services;

public interface IStyleDefaultProductsProvider
{
    Task<List<DefaultProductsDto>> GetDefaultProducts(string styleId="Basic.Berlin");
    Task<List<ProductCategoryDto>> GetDefaultProductsForRoom(string roomType, string styleId);
}

public record DefaultProductsDto
{
    public string RoomType { get; set; }
    public List<ProductCategoryDto> DefaultProducts { get; set; }
}

public class ProductCategoryDto
{
    public string? ProductId { get; set; }
    public string CategoryName { get; set; }
    
    public ProductSource ProductSource { get; set; }
}

