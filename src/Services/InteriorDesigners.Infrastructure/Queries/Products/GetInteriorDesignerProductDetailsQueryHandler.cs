using BuildingBlocks.Abstractions.CQRS.CQRS;
using BuildingBlocks.Application.Exceptions.Exceptions;
using BuildingBlocks.Infrastructure.DataAccess.Cosmos.Extensions;
using InteriorDesigners.Application.Products.Dtos;
using InteriorDesigners.Application.Products.GetProducts;
using InteriorDesigners.Domain.InteriorDesigner;
using InteriorDesigners.Infrastructure.DataAccess;

namespace InteriorDesigners.Infrastructure.Queries.Products;

public class GetInteriorDesignerProductDetailsQueryHandler : IQueryHandler<GetInteriorDesignerProductDetailsQuery, GetInteriorDesignerProductDetailsQueryResponse>
{
    private readonly InteriorDesignerContext _interiorDesignerContext;
    private readonly ImageStorageSettings _imageStorageSettings;


    public GetInteriorDesignerProductDetailsQueryHandler(InteriorDesignerContext interiorDesignerContext, ImageStorageSettings imageStorageSettings)
    {
        _interiorDesignerContext = interiorDesignerContext;
        _imageStorageSettings = imageStorageSettings;
    }

    public async Task<GetInteriorDesignerProductDetailsQueryResponse> Handle(GetInteriorDesignerProductDetailsQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.InteriorDesigner))
            throw new CustomException("Interior Designer is required");

        if (string.IsNullOrWhiteSpace(request.Id))
        {
            throw new CustomException("ProductId is required");
        }

        var baseQuery = _interiorDesignerContext.Query<ProductAggregate>()
            .Where(x => x.InteriorDesignerCode == request.InteriorDesigner);

        if (request.FilterByCoreProductId)
        {
            baseQuery = baseQuery.Where(x => x.CoreProductData.Id == request.Id);
           
        }
        else
        {
            baseQuery = baseQuery.Where(x => x.Id == request.Id);
        }

        baseQuery = baseQuery.Where(x => x.IsDeleted == false || x.IsDeleted == null);
        
        var product = (await baseQuery.ToDocumentListAsync()).Result.SingleOrDefault();

        if (product == null)
        {
            throw new CustomException("Product not found");
        }

        var productModel = new InteriorDesignerProductDto
        {
            Id = product.Id,
            InteriorDesignerCode = product.InteriorDesignerCode,
            CoreProductData = new CoreProductDataDto
            {
                Name = product.CoreProductData.Name,
                Categories = product.CoreProductData.Categories,
                Category = new CategoryDto
                {
                    Name = product.CoreProductData.Category.Type,
                    DisplayName = product.CoreProductData.Category.DisplayName
                },
                Unit = product.CoreProductData.Unit,
                SubCategories = product.CoreProductData.SubCategories,
                ThumbnailUrl = ThumbnailUrlHelper.GetUrl(product.Id, _imageStorageSettings)
            },
            DisplayName = product.DisplayName,
            Description = product.Description,
            Price = product.Price,
            Images = product.Images?.Select(x => new ProductImageDto
            {
                FileName = x.FileName
            }).ToList(),
            IsDeleted = product.CoreProductData.IsDeleted.HasValue && product.CoreProductData.IsDeleted.Value
                                                                   && product.IsDeleted.HasValue &&
                                                                   product.IsDeleted.Value,
            IsEnabled = product.IsEnabled,
            ShowLabel = product.ShowLabel,
            Supplier = new SupplierDto
            {
                Contact = new ContactDto
                {
                    Email = product?.Supplier?.Contact.Email,
                    PhoneNumber = product?.Supplier?.Contact.PhoneNumber
                },
                Link = product?.Supplier?.Link,
                Logo = product?.Supplier?.Logo,
                Name = product?.Supplier?.Name,
                ProductLink = product?.Supplier?.ProductLink
            }
        };

        return new GetInteriorDesignerProductDetailsQueryResponse
        {
            Product = productModel
        };
    }
}