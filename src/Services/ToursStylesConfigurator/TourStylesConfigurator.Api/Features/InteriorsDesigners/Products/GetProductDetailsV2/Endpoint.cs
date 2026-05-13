using Microsoft.AspNetCore.Authorization;

namespace TourStylesConfigurator.Api.Features.InteriorsDesigners.Products.GetProductDetailsV2;

[HttpGet("products/{Id}/interiordesigners")]
[AllowAnonymous]
public class Endpoint : Endpoint<Request, GetProductDetailResponse2>
{
    private readonly IInteriorDesignerServiceClient _interiorDesignerServiceClient;


    public Endpoint(IInteriorDesignerServiceClient interiorDesignerServiceClient)
    {
        _interiorDesignerServiceClient = interiorDesignerServiceClient;
    }

    public override async Task HandleAsync(Request request, CancellationToken c)
    {
        
        var result = await _interiorDesignerServiceClient.GetProductDetailWithInteriorDesingersAsync(
            request.Id,HttpContext.GetLanguageFromHeader());
        
        await SendAsync(result, cancellation: c);
    }
}