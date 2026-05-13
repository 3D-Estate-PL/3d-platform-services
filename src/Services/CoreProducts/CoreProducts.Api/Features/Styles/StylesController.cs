using CoreProducts.Application.Styles.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CoreProducts.Api.Features.Styles;

[AllowAnonymous]
[Route("api/[controller]")]
public class StylesController : ControllerBase
{
    private readonly IMediator _mediator;

    public StylesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("DefaultProducts")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetStyleDefaultProductsResponse))]
    public async Task<GetStyleDefaultProductsResponse> GetStyleDefaultProducts(
        CancellationToken c)
    {
        return await _mediator.Send(new GetStyleDefaultProductsQuery(), c);
    }
}