namespace TourStylesConfigurator.Api.Features.InteriorsDesigners.Products.GetProductDetails;

public class CategoryProductsDto
{
    public string SubCategoryName { get; set; }
    
    public string SubCategoryDisplayName { get; set; }
    public List<InteriorDesignerProductDto> Products { get; set; }
}


public class InteriorDesignerProductDto
{
    public string Id { get; set; }

    public CoreProductDataDto CoreProductData { get; set; }

    public bool? IsDeleted { get; set; }

    public bool IsEnabled { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }

    public List<ProductImageDto>? Images { get; set; }

    public bool ShowLabel { get; set; }

    public SupplierDto Supplier { get; set; }
    
    public decimal? Price { get; set; }
    public string InteriorDesignerCode { get; set; }
}

public class CoreProductDataDto
{
    public string Name { get; set; }

    public string Unit { get; set; }

    public CategoryDto Category { get; set; }

    public IList<string> Categories { get; set; }

    public IList<string> SubCategories { get; set; }
    
    public string ThumbnailUrl { get; set; }
}


public class CategoryDto
{
    private string _name;
    public string Name
    {
        get => _name;
        set => _name = System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(value);
    }
    public string DisplayName { get; set; }
}

public class SupplierDto
{
    public string? Name { get; set; }
    public string? Link { get; set; }
    public string? Logo { get; set; }
    public string? ProductLink { get; set; }
    public ContactDto? Contact { get; set; }
}

public class ContactDto
{
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}

public class ProductImageDto
{
    public string FileName { get; set; }
}