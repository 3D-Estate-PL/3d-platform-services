using System.Runtime.InteropServices;
using TourStylesConfigurator.Api.Features.StyleConfigurations.Services;
using TourStylesConfigurator.Api.Features.StyleConfigurations.Services.Commons;

namespace TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Styles;

public class RoomStyle
{
    public TourStyle BaseStyle { get; set; }
    public bool IsCustom { get; set; }

    public static RoomStyle New(TourStyle style)
    {
        return new RoomStyle
        {
            BaseStyle = style
        };
    }

    public static RoomStyle Custom(TourStyle baseStyle)
    {
        var style = New(baseStyle);
        style.IsCustom = true;
        return style;
    }
}

public class RoomItem
{
    public string Id { get; set; }
    public string Type { get; set; }
    public string CustomName { get; set; }
    public RoomStyle SelectedStyle { get; set; }
    public List<ProductItem> BaseStyleProducts {  get;    set; } = new List<ProductItem>();
    
    public List<ProductItem> GetProducts() => SelectedStyle.IsCustom ? CustomStyleProducts : BaseStyleProducts;
    public List<ProductItem> CustomStyleProducts {  get;    set; } = new List<ProductItem>();
    
    public TourStyle? CustomStyle { get; set; }

    public bool IsRequired { get; set; }
    
    public bool IsDefinedByUser { get; set; }
    public bool AllowOverrideBaseConfiguration { get; set; }
    
    public static RoomItem New(string customName, string roomType, 
        RoomStyle roomStyle,
        List<ProductItem> defaultProducts,
        string? id = null, 
        bool? isRequired = false,
        bool? isDefinedByUser = true)
    {
        return new RoomItem
        {
            Id = id ?? Guid.NewGuid().ToString(),
            CustomName = customName,
            SelectedStyle = roomStyle,
            Type = roomType,
            BaseStyleProducts = defaultProducts,
            IsRequired = isRequired ?? false,
            IsDefinedByUser = isDefinedByUser ?? true
        };
    }

    public void SetBaseStyle(TourStyle style, List<ProductItem> defaultProducts)
    {
        SelectedStyle = RoomStyle.New(style);
        BaseStyleProducts = defaultProducts;
    }
    
    public void SetCustomStyle()
    {
        if (CustomStyle == null)
        {
            throw new Exception("Cannot change to custom style because is do not exists.");
        }
        
        SelectedStyle = RoomStyle.Custom(CustomStyle);
    }

    public ProductItem GetSelectedProductForCategory(string categoryName)
    {
        var product = GetProducts().SingleOrDefault(x => x.CategoryName.ToLower() == categoryName.ToLower());
        return product;
    }
    
    public void ChangeProduct(string categoryName, string productId, List<ProductCategoryDto> defaultProductsForStyles)
    {
        if (SelectedStyle.IsCustom == false)
        {
            SelectedStyle.IsCustom = true;
            CustomStyle = SelectedStyle.BaseStyle;
            CustomStyleProducts = BaseStyleProducts.Clone();
        }
        
        if (SelectedStyle.IsCustom)
        {
            var product = GetProductForCategory();
            product.ProductId = productId;
            product.IsOverridden = IsRollbackToDefaultStyleProduct();

            if (CustomStyleProducts.All(x => x.IsOverridden == false))
            {
                SelectedStyle.IsCustom = false;
                CustomStyleProducts = null;
                CustomStyle = null;
                BaseStyleProducts = defaultProductsForStyles.Select(x=> new ProductItem
                {
                    IsOverridden = false,
                    CategoryName = x.CategoryName,
                    ProductId = x.ProductId,
                    ProductSource = x.ProductSource
                }).ToList();
            }
        }

        bool IsRollbackToDefaultStyleProduct()
        {
            return defaultProductsForStyles.
                SingleOrDefault(x => string.Equals(x.CategoryName,categoryName,StringComparison.OrdinalIgnoreCase))
                ?.ProductId != productId;
        }

        ProductItem GetProductForCategory()
        {
            var product = CustomStyleProducts?.SingleOrDefault(x =>
                string.Equals(x.CategoryName, categoryName, StringComparison.OrdinalIgnoreCase));
            if (product == null)
            {
                throw new Exception($"Product for category {categoryName} not found.");

            }
            
            return product;
        }
    }

    public RoomItem Clone(string? customName = null)
    {
        var newRoom = New(customName ?? CustomName, Type, SelectedStyle, new List<ProductItem>());
       
        if (SelectedStyle.IsCustom)
        {
            if (newRoom.CustomStyleProducts != null)
            {
                newRoom.CustomStyleProducts = CustomStyleProducts.Select(x => new ProductItem
                {
                    CategoryName = x.CategoryName,
                    ProductId = x.ProductId

                }).ToList();

                newRoom.IsRequired = IsRequired;
            }   
        }
        else
        {
            if (newRoom.BaseStyleProducts != null)
            {
                newRoom.BaseStyleProducts = BaseStyleProducts.Select(x => new ProductItem
                {
                    CategoryName = x.CategoryName,
                    ProductId = x.ProductId

                }).ToList();
            }   
        }

        return newRoom;
    }
}