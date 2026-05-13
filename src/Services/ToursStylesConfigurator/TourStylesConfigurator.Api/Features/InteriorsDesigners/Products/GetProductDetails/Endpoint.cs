using Microsoft.AspNetCore.Authorization;

namespace TourStylesConfigurator.Api.Features.InteriorsDesigners.Products.GetProductDetails;

[HttpGet("products/{Id}")]
[AllowAnonymous]
public class Endpoint : Endpoint<Request, GetProductDetailResponse>
{
    private readonly IInteriorDesignerServiceClient _interiorDesignerServiceClient;

    public Endpoint(IInteriorDesignerServiceClient interiorDesignerServiceClient)
    {
        _interiorDesignerServiceClient = interiorDesignerServiceClient;
    }

    public override async Task HandleAsync(Request request, CancellationToken c)
    {
        if (request.InteriorDesigner == null)
        {
            request.InteriorDesigner = "Decoroom";//TODO remove
        }

        var result = await _interiorDesignerServiceClient.GetAsync(request.InteriorDesigner,
            request.Id, HttpContext.GetLanguageFromHeader());
        
        await SendAsync(result, cancellation: c);

    }
}