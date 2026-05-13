using InteriorDesigners.Application.Products.Dtos;

namespace InteriorDesigners.Application.Services;

public class CoreProductModel
{
    public string Id { get; set; }
    
    public string Name { get; set; }

    public string Unit { get; set; }

    public CategoryDto Category { get; set; }

    public IList<string> Categories { get; set; }

    public IList<string> SubCategories { get; set; }
    
    public string ThumbnailUrl { get; set; }
    
    public List<string> Designers { get; set; } = new List<string>();   
    
}