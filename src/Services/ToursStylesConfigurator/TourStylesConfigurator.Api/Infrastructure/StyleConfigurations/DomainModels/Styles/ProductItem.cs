namespace TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Styles;

public class ProductItem : ICloneable
{
    public string? ProductId { get; set; }
    
    public string CategoryName { get; set; }
    
    public bool IsOverridden { get; set; }
    public ProductSource ProductSource { get; set; }
    public object Clone()
    {
        return new ProductItem
        {
            CategoryName = CategoryName,
            IsOverridden = IsOverridden,
            ProductId = ProductId,
            ProductSource = ProductSource
        };
    }
}


public enum ProductSource
{
    RoomStyle,
    CommonStyle
}