using BuildingBlocks.Domain.DDD;

namespace InteriorDesigners.Domain.InteriorDesigner;

public class ProductAggregate : IDocument
{
    public string Id { get; set; }

    public string InteriorDesignerCode { get; set; }
    
    public CoreProductData CoreProductData { get; set; }
    
    public bool? IsDeleted { get; set; }

    public bool IsEnabled { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }
    
    public bool ShowLabel { get; set; }
    
    public decimal? Price { get; set; }
    
    public Supplier? Supplier { get; set; }
    
    public List<ProductImage> Images { get; set; }


    public ProductAggregate()
    {
        Supplier = new Supplier
        {
            Contact = new Contact()
        };
        Images = new List<ProductImage>();
    }

    public DocumentIdentity GetIdentity()
    {
        return new ProductIdentity(Id,InteriorDesignerCode);
    }
}

public class CoreProductData
{
    public string Name { get; set; }

    public string Unit { get; set; }

    public ProductCategory Category{ get; set; }

    public IList<string> Categories { get; set; }

    public IList<string> SubCategories { get; set; }
    
    public bool? IsDeleted { get; set; }
    
    public string Id { get; set; }
}

public class ProductCategory
{
    public string Type { get; set; }
    public string DisplayName { get; set; }
}

public class Supplier
{
    public string? Name { get; set; }
    public string? Link { get; set; }
    public string? Logo { get; set; }
    public string? ProductLink { get; set; }
    public Contact? Contact { get; set; }
}

public class Contact
{
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}

public class ProductImage
{
    public string FileName { get; set; }
}