using Microsoft.AspNetCore.Authorization;
using TourStylesConfigurator.Api.Features.InteriorsDesigners.Products.GetProductDetails;

namespace TourStylesConfigurator.Api.Features.InteriorsDesigners.Products.GetProducts;

[HttpGet("products")]
[AllowAnonymous]
public class Endpoint : Endpoint<GetProductsRequest, GetProductsResponse>
{
    private readonly IInteriorDesignerServiceClient _interiorDesignerServiceClient;
    private readonly ICategoryProvider _categoryProvider;


    public Endpoint(IInteriorDesignerServiceClient interiorDesignerServiceClient, ICategoryProvider categoryProvider)
    {
        _interiorDesignerServiceClient = interiorDesignerServiceClient;
        _categoryProvider = categoryProvider;
    }

    public override async Task HandleAsync(GetProductsRequest request, CancellationToken c)
    {
        if (string.IsNullOrEmpty(request.InteriorDesigner))
        {
            request.InteriorDesigner = InteriorDesignerContextProvider.GetContext(HttpContext);
        }

        var filters = new List<KeyValuePair<string, string>>
        {
             KeyValuePair.Create("Category", request.Category)!,
             KeyValuePair.Create("InteriorDesigner", request.InteriorDesigner)!,
        };

        foreach (var id in request.Ids ?? new List<string>())
        {
            filters.Add(KeyValuePair.Create<string, string>("Ids",id));
        }
        
        
        var result = await _interiorDesignerServiceClient.GetProductsAsync(filters,
            HttpContext.GetLanguageFromHeader());

        if (request.Category != null)
        {
               result.Category = new CategoryDto()
                {
                    Name = request.Category,
                    DisplayName = await _categoryProvider.GetDisplayNameForCategoryType(request.Category.Split(".").Last(),HttpContext.GetLanguageFromHeader())
                };
        }
        
        await SendAsync(result, cancellation: c);
    }
}