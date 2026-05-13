using InteriorDesigners.Application.IntegrationEvents.Events;
using InteriorDesigners.Application.Products.Dtos;
using InteriorDesigners.Application.Services;
using InteriorDesigners.Domain.InteriorDesigner;
using NameGenerator.Generators;
namespace InteriorDesigners.Application.IntegrationEvents.EventsHandlers;

public static class Mapper
{
    public static ProductAggregate Map(this CoreProductModel coreProductDto, ProductAggregate product,
        string interiorDesigner)
    {
        var realNameGenerator = new RealNameGenerator();
        product.InteriorDesignerCode = interiorDesigner;
        product.CoreProductData = new CoreProductData
        {
            Id = coreProductDto.Id,
            Name = coreProductDto.Name,
            Category = new ProductCategory
            {
                Type = coreProductDto.Category.Name,
                DisplayName = coreProductDto.Category.DisplayName,
            },
            Unit = coreProductDto.Unit,
            Categories = coreProductDto.Categories,
            SubCategories = coreProductDto.SubCategories
        };

        if (string.IsNullOrEmpty(product.Id))
        {
            product.Id = coreProductDto.Id;
        }
        return product;
    }

    private static double GetRandomNumber(double minimum, double maximum)
    { 
        var random = new Random();
        return random.NextDouble() * (maximum - minimum) + minimum;
    }
}