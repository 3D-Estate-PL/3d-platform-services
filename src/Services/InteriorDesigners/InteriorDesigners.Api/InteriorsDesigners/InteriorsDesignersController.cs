using InteriorDesigners.Api.InteriorsDesigners.Requests;
using InteriorDesigners.Application.InteriorsDesigners.Commands;
using InteriorDesigners.Application.InteriorsDesigners.Commands.Presets;
using InteriorDesigners.Application.InteriorsDesigners.Queries.GetAll;
using InteriorDesigners.Application.Products.ExportProductsToGoogleSheet;
using InteriorDesigners.Application.Products.GetProducts;
using InteriorDesigners.Application.Products.ImportProducts;
using InteriorDesigners.Domain.InteriorDesigner;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InteriorDesigners.Api.InteriorsDesigners;

[AllowAnonymous]
[Route("api/[controller]")]
public class InteriorsDesignersController : ControllerBase
{
    private readonly IMediator _mediator;

    public InteriorsDesignersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetInteriorsDesignersQueryResponse))]
    public async Task<ActionResult<GetInteriorsDesignersQueryResponse>> GetAllAsync(
        CancellationToken c)
    {
        return await _mediator.Send(new GetInteriorsDesignersQuery(), c);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> AddInteriorDesignerAsync([FromBody] AddNewInteriorDesignerRequest request,
        CancellationToken c)
    {
        await _mediator.Send(new AddNewInteriorDesignerCommand
        {
            Name = request.Name,
            DisplayName = request.DisplayName,
            ProductsExternalLink = request.ProductExternalLink
        }, c);
        return Accepted();
    }
    
    [HttpPut(template:"{code}",Name = nameof(UpdateInteriorDesignerAsync))]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> UpdateInteriorDesignerAsync(string code,
        [FromBody] UpdateInteriorDesignerRequest request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateInteriorDesignerCommand
        {
            Code = code,
            DisplayName = request.DisplayName,
            ProductExternalLink = request.ProductExternalLink
        }, cancellationToken);
        return Ok();
    }
    
    
    [HttpDelete(template:"{code}", Name = nameof(DeleteInteriorDesignerAsync))]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<ActionResult> DeleteInteriorDesignerAsync(string code,
        CancellationToken c)
    {
        await _mediator.Send(new DeleteInteriorDesignerCommand
        {
            Code = code
        }, c);
        return Accepted();
    }
    
    
    /// <summary>
    /// Update  Styles
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPut(template:"{code}/styles",Name = nameof(UpdateInteriorDesignerStyles))]
    public async Task<IActionResult> UpdateInteriorDesignerStyles(string code,[FromBody] UpdateInteriorDesignerStyleRequest request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateInteriorDesignerStylesCommand
        {
            Code = code,
            Styles = request.Styles
        }, cancellationToken);
        return Ok();
    }
    
            
    /// Add Tour Info Preset
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost(template:"{code}/presets",Name = nameof(AddTourInfoPresets))]
    public async Task<IActionResult> AddTourInfoPresets(string code, 
        [FromBody] AddTourInfoPresetRequest request)
    {
        await _mediator.Send(new AddTourInfoPresetCommand()
        {
            InteriorDesignerCode = code,
            Name = request.Name,
            IsDefault = request.IsDefault,
            ExternalLink = request.ExternalLink
        });
        return Ok();
    }
    
    
    /// Update Tour Info Preset
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPut(template:"{code}/presets/{name}",Name = nameof(UpdateTourInfoPresets))]
    public async Task<IActionResult> UpdateTourInfoPresets(string code, string name, 
        [FromBody] UpdateTourInfoPresetRequest request)
    {
        await _mediator.Send(new UpdateTourInfoPresetCommand()
        {
            InteriorDesignerCode = code,
            Name = name,
            IsDefault = request.IsDefault,
            ExternalLink = request.ExternalLink
        });
        return Ok();
    }
    
        
    /// Delete Tour Info Preset
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpDelete(template:"{code}/presets/{name}",Name = nameof(DeleteTourInfoPresets))]
    public async Task<IActionResult> DeleteTourInfoPresets(string code, string name)
    {
        await _mediator.Send(new DeleteTourInfoPresetCommand()
        {
            InteriorDesignerCode = code,
            Name = name,
        });
        return Ok();
    }
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost(template:"{code}/presets/{name}/import",Name = nameof(ImportTourInfoPresets))]
    public async Task<IActionResult> ImportTourInfoPresets(string code, string name)
    {
        await _mediator.Send(new ImportTourPresetFromFileCommand()
        {
            InteriorDesignerCode = code,
            Name = name,
        });
        return Ok();
    }
    
    /// <summary>
    /// Get interior designer tour info presets
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet(template:"{code}/presets",Name = nameof(GetTourInfoPresets))]
    public async Task<GetInteriorDesignerTourInfoPresetsQueryResult> GetTourInfoPresets(string code)
    {
        return await _mediator.Send(new GetInteriorDesignerTourInfoPresetsQuery
        {
            InteriorDesignerCode = code
        });
    }
    
    
    /// <summary>
    /// Get interior designer tour info preset
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet(template:"{code}/presets/{name}",Name = nameof(GetTourInfoPreset))]
    public async Task<GetInteriorDesignerTourInfoPresetQueryResult> GetTourInfoPreset(string code, string name)
    {
        return await _mediator.Send(new GetInteriorDesignerTourInfoPresetQuery
        {
            InteriorDesignerCode = code,
            Name = name
            
        });
    }

    
    
    [HttpGet(template:"{code}/products",Name = nameof(GetProductsAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProductsQueryResponse))]
    public async Task<ActionResult<GetProductsQueryResponse>> GetProductsAsync(string code,
        GetProductsRequest request,
        CancellationToken c)
    {
        return await _mediator.Send(new GetInteriorDesignerProductsQuery
        {
            Ids = request.Ids,
            InteriorDesigner = code,
            Category = request.Category,
            Language = HttpContext.GetLanguageFromHeader()
        }, c);
    }
    
    [HttpGet(template:"{interiordesigner}/products/{id}",Name = nameof(GetInteriorDesignerProductDetailsAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetInteriorDesignerProductDetailsQueryResponse))]
    public async Task<ActionResult<GetInteriorDesignerProductDetailsQueryResponse>> GetInteriorDesignerProductDetailsAsync(string interiordesigner,
        string id,
        GetProductDetailsRequest request,
        CancellationToken c)
    {
        return await _mediator.Send(new GetInteriorDesignerProductDetailsQuery
        {
            Id = id,
            InteriorDesigner = interiordesigner,
            FilterByCoreProductId = request.FilterByCoreProductId
        }, c);
    }
    
    
    [HttpPost("{code}/import-from-google-sheet")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> ImportFromGoogleSheetAsync(string code,
        CancellationToken c)
    {
        await _mediator.Send(new ImportProductsFromGoogleSheetCommand
        {
            InteriorDesignerCode = code
        }, c);
        return Accepted();
    }
    
    [HttpPost("{code}/export-to-google-sheet")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> ExportToGoogleSheetAsync(string code,
        CancellationToken c)
    {
        await _mediator.Send(new ExportProductsToGoogleSheetCommand
        {
            InteriorDesignerCode = code
        }, c);
        return Accepted();
    }
    

}