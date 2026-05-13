using CoreProducts.Application.Products.Commands;
using CoreProducts.Application.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CoreProducts.Api.Features.Products;

[AllowAnonymous]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator;
    }
    
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [HttpPost("import")]
    public async Task<ActionResult> ImportProductFromFileAsync([FromBody]ImportProductsFromFileCommand command,
        CancellationToken c)
    {
        await _mediator.Send(command, c);
        return Accepted();
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProductsResponse))]
    public async Task<ActionResult<GetProductsResponse>> GetProductsAsync([FromQuery]GetProductsQuery getProductsQuery,
        CancellationToken c)
    {
        return await _mediator.Send(getProductsQuery, c);
    }
}