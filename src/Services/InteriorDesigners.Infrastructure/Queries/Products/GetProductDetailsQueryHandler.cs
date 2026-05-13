using BuildingBlocks.Abstractions.CQRS.CQRS;
using BuildingBlocks.Application.Exceptions.Exceptions;
using InteriorDesigners.Application.Products.Dtos;
using InteriorDesigners.Application.Products.GetProducts;
using InteriorDesigners.Domain.InteriorDesigner;
using InteriorDesigners.Infrastructure.DataAccess;

namespace InteriorDesigners.Infrastructure.Queries.Products;

public class GetProductDetailsQueryHandler : IQueryHandler<GetProductDetailsQuery,
    GetProductDetailResponse>
{
    private readonly InteriorDesignerContext _interiorDesignerContext;


    public GetProductDetailsQueryHandler(InteriorDesignerContext interiorDesignerContext)
    {
        _interiorDesignerContext = interiorDesignerContext;
    }

    public async Task<GetProductDetailResponse> Handle(GetProductDetailsQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Id)) throw new CustomException("ProductId is required");


        var query = $"SELECT distinct VALUE c FROM c WHERE c.document.id = \"{request.Id}\"";
        var interiorDesignerProducts = _interiorDesignerContext.FromSqlRaw<ProductAggregate>(query)
            .Result.ToList();
        
        var interiorDesignerQuery = $"SELECT distinct VALUE c FROM c";
        var interiorDesigners = _interiorDesignerContext.FromSqlRaw<InteriorDesignerAggregate>(interiorDesignerQuery)
            .Result.ToList();

        var response = new GetProductDetailResponse
        {
            Id = interiorDesignerProducts.FirstOrDefault()?.Id,
            Name = interiorDesignerProducts.FirstOrDefault()?.CoreProductData.Name,
            Type = interiorDesignerProducts.FirstOrDefault()?.CoreProductData.Category.Type,
            Categories = interiorDesignerProducts.FirstOrDefault()?.CoreProductData.Categories,
            SubCategories = interiorDesignerProducts.FirstOrDefault()?.CoreProductData.SubCategories,
            Unit = interiorDesignerProducts.FirstOrDefault()?.CoreProductData.Unit,
            Offers = new List<InteriorDesignerProductDto>(),
            Styles = new List<string>()
        };

        foreach (var product in interiorDesignerProducts)
        {
            var interiorDesignerProductDto = new InteriorDesignerProductDto
            {
                Id = product.Id,
                InteriorDesigner = product.InteriorDesignerCode,
                InteriorDesignerCode = product.InteriorDesignerCode,
                DisplayName = product.DisplayName,
                Description = product.Description,
                Price = product.Price,
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
            if (interiorDesigners.Any(x => x.Code == interiorDesignerProductDto.InteriorDesignerCode))
            {
                response.Offers.Add(interiorDesignerProductDto);
            }
        }


        return response;
    }
}