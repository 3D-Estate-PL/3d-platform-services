using System.Text.Json;
using BuildingBlocks.Abstractions.CQRS.CQRS;
using BuildingBlocks.Infrastructure.DataAccess.Cosmos.Extensions;
using InteriorDesigners.Application.Products.Dtos;
using InteriorDesigners.Application.Products.GetProducts;
using InteriorDesigners.Domain.InteriorDesigner;
using InteriorDesigners.Infrastructure.DataAccess;

namespace InteriorDesigners.Infrastructure.Queries.Products;

public class GetInteriorDesignerProductsQueryHandler : IQueryHandler<GetInteriorDesignerProductsQuery, GetProductsQueryResponse>
{
    private readonly InteriorDesignerContext _interiorDesignerContext;
    private readonly IProductSubCategoriesService _productSubCategoriesService;
    private readonly ImageStorageSettings _imageStorageSettings;
    public GetInteriorDesignerProductsQueryHandler(InteriorDesignerContext interiorDesignerContext, IProductSubCategoriesService productSubCategoriesService, ImageStorageSettings imageStorageSettings)
    {
        _interiorDesignerContext = interiorDesignerContext;
        _productSubCategoriesService = productSubCategoriesService;
        _imageStorageSettings = imageStorageSettings;
    }

    public async Task<GetProductsQueryResponse> Handle(GetInteriorDesignerProductsQuery request, CancellationToken cancellationToken)
    {
       
         var ids = request.Ids;

        List<ProductAggregate> products;
        
        if (ids != null && string.IsNullOrEmpty(request.InteriorDesigner))
        {
            products = (await _interiorDesignerContext.Query<ProductAggregate>()
                .Where(x => ids.Contains(x.Id) && x.IsEnabled).ToDocumentListAsync()).Result;
        }
        else if (ids != null && string.IsNullOrEmpty(request.InteriorDesigner) == false)
        {
            products =  ( await _interiorDesignerContext.Query<ProductAggregate>().Where(x => ids.Contains(x.Id) && x.IsEnabled && x.InteriorDesignerCode == request.InteriorDesigner).
                ToDocumentListAsync()).Result;
        }
        else
        {
           
            var query = "";
            if (request.Category != null)
            {
                query =
                    $"SELECT distinct VALUE c FROM c  join t in c.document.coreProductData.categories where c.document.isEnabled = true and StringEquals(t,\"{request.Category}\", true) and c.document.interiorDesignerCode=\"{request.InteriorDesigner}\"";
                products = await _interiorDesignerContext.FromSqlRaw<ProductAggregate>(query);
            }
            else
            {
                products = new List<ProductAggregate>(); 
            }
        }
        
        var productsByCategories = products
            .SelectMany(product =>
            {
                if (product.CoreProductData.SubCategories.Any())
                {
                    return product.CoreProductData.SubCategories.Select(category =>
                        new
                        {
                            Category = category,
                            Product = product
                        });

                }
                else
                {
                    return new[]
                    {
                        new
                        {
                            Category = "Other",
                            Product = product
                        }
                    };
                }
            })
            .GroupBy(t => t.Category).
            ToDictionary(grouping => grouping.Key, grouping => grouping.Select(y=>y.Product));


        var language = request.Language;
        var subCategories = await _productSubCategoriesService.GetAll(language);

        var productsModel = productsByCategories.Select(y => new CategoryProductsDto
        {
            SubCategoryName = JsonNamingPolicy.CamelCase.ConvertName(y.Key),
            SubCategoryDisplayName = subCategories.FirstOrDefault(x=>string.Equals(x.Name,y.Key,StringComparison.OrdinalIgnoreCase))?.DisplayName,
            Products = y.Value.Select(product => new InteriorDesignerProductDto
            {
                    
                Id = product.Id,
                InteriorDesignerCode = product.InteriorDesignerCode,
                CoreProductData = new CoreProductDataDto()
                {
                    Name = product.CoreProductData.Name,
                    Categories = product.CoreProductData.Categories,
                    Category =new CategoryDto
                    {
                        Name =  product.CoreProductData.Category.Type,
                        DisplayName =  product.CoreProductData.Category.DisplayName
                    },                    Unit = product.CoreProductData.Unit,
                    SubCategories = product.CoreProductData.SubCategories,
                    ThumbnailUrl =  ThumbnailUrlHelper.GetUrl(product.Id,_imageStorageSettings)
                },
                DisplayName = product.DisplayName,
                Description = product.Description,
                Price = product.Price,
                Images = product.Images?.Select(x=> new ProductImageDto()
                {
                    FileName = x.FileName
                }).ToList(),
                IsDeleted = (product.CoreProductData.IsDeleted.HasValue && product.CoreProductData.IsDeleted.Value) 
                            && (product.IsDeleted.HasValue  && product.IsDeleted.Value),
                IsEnabled = product.IsEnabled,
                ShowLabel = product.ShowLabel,
                Supplier = new SupplierDto
                {
                    Contact = new ContactDto
                    {
                        Email =  product?.Supplier?.Contact.Email,
                        PhoneNumber =  product?.Supplier?.Contact.PhoneNumber
                    },
                    Link = product?.Supplier?.Link,
                    Logo = product?.Supplier?.Logo,
                    Name = product?.Supplier?.Name,
                    ProductLink = product?.Supplier?.ProductLink,
                }}).ToList()

        }).ToList();
            


        return new GetProductsQueryResponse(productsModel);
    }
}