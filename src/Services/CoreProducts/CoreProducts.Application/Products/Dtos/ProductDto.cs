using CoreProducts.Application.Products.Services;
using CoreProducts.Domain.Products;

namespace CoreProducts.Application.Products.Dtos;

public class ProductDto
{
    public required string Id { get; set; }

    public required  string Name { get; set; }

    public required  string Unit { get; set; }

    public required  string Type { get; set; }

    public List<string> Categories { get; set; } = new List<string>();

    public List<string> SubCategories { get; set; } = new List<string>();

    public List<string> Designer { get; set; } = new List<string>();

    public bool? IsDeleted { get; set; }

    public async Task<Product> MapToDomainModel(string id, 
        ICategoryProvider categoryProvider)
    {
        var product = new Product
        {
            Id = id,
            Name = Name,
            Unit = Unit,
            Categories = Categories,
            SubCategories = SubCategories,
            IsDeleted = IsDeleted,
            Category = new ProductCategory
            {
                Type = Type, 
                DisplayName = await categoryProvider.GetDisplayNameForCategoryType(Type)
            },
            Designers = Designer
        };

        return product;
    }
}