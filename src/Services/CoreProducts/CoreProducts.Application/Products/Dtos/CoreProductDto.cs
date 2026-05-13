namespace CoreProducts.Application.Products.Dtos;

public class CoreProductDto
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public required string Unit { get; init; }
    
    public required CategoryDto Category { get; set; }

    public required IList<string> Categories { get; set; } = new List<string>();

    public required IList<string> SubCategories { get; set; } = new List<string>();
    public string? ThumbnailUrl { get; set; }
    
    public List<string> Designers { get; set; } = new List<string>();
}
