using BuildingBlocks.Domain.DDD;

namespace CoreProducts.Domain.Products;

public class Product : IDocument
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public required string Unit { get; set; }
    
    public required ProductCategory Category { get; set; }

    public required List<string> Categories { get; set; } = new List<string>();

    public required List<string> SubCategories { get; set; } = new List<string>();
    
    public bool? IsDeleted { get; set; }
    public List<string> Designers { get; set; } = new List<string>();

    public DocumentIdentity GetIdentity()
    {
        return new CoreProductIdentity(Id);
    }
}