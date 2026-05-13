using InteriorDesigners.Api.InteriorsDesigners;
using InteriorDesigners.Application.Products.ExportProductsToGoogleSheet;
using InteriorDesigners.Application.Products.GetProducts;
using InteriorDesigners.Application.Products.ImportProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InteriorDesigners.Api.Products;

[AllowAnonymous]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProductsQueryResponse))]
    public async Task<ActionResult<GetProductsQueryResponse>> GetProductsAsync(GetProductsRequest request,
        CancellationToken c)
    {
        return await _mediator.Send(new GetInteriorDesignerProductsQuery
        {
            Ids = request.Ids,
            Category = request.Category,
            InteriorDesigner = request.InteriorDesigner ?? HttpContext.GetInteriorDesignerContext(),
            Language = HttpContext.GetLanguageFromHeader()
        }, c);
    }
    
    [HttpGet(template:"{id}",Name = nameof(GetProductDetailsAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetInteriorDesignerProductDetailsQueryResponse))]
    public async Task<Application.Products.GetProducts.GetProductDetailResponse> GetProductDetailsAsync(
        string id,
        CancellationToken c)
    {
        return await _mediator.Send(new GetProductDetailsQuery()
        {
            Id = id
        }, c);
    }
}