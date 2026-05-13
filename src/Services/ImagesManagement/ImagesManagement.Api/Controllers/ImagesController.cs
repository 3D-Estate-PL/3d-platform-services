using BuildingBlocks.Application.EventBus;
using ImagesManagement.Application.Images;
using ImagesManagement.Application.IntegrationEvents;
using ImagesManagement.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ImagesManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IEventBus _bus;

        public ImagesController(IMediator mediator, IEventBus bus)
        {
            _mediator = mediator;
            _bus = bus;
        }

        [HttpGet("{storageKey}")]
        public async Task<IActionResult> GetImageByQuery(string storageKey,
            string imageName,
            int? width, int? height,int? maxRes,  int? compression, string? destinationFileName)
        {
            var extension = Helper.GetExtension(destinationFileName, imageName);
            var fileEncoder = Helper.GetDefaultEncoder(Path.GetExtension(extension));
            destinationFileName = Helper.GetDestinationFileName(destinationFileName, fileEncoder);
            
            var command = new ResizeImageCommand(storageKey,imageName,width,height,maxRes, fileEncoder, compression ?? 90, destinationFileName);
            var result = await _mediator.Send(command);

            if (result.FileUrl == null)
            {
                return NotFound();
            }
            
            return Redirect(result.FileUrl);
        }
        
        [HttpGet("{storageKey}/{imageName}__{width}x{height}.{extension}")]
        public async Task<IActionResult> GetImagePath(string storageKey, string imageName,
            int width, int height, string extension, int? compression,Encoder? encoder)
        {
            var query = new ResizeImageCommand(storageKey,$"{imageName}.{extension}",width,height,null, encoder?? Helper.GetDefaultEncoder(extension), compression ?? 90);
            
            var result = await _mediator.Send(query);
            
            if (result.FileUrl == null)
            {
                return NotFound();
            }
            
            return Redirect(result.FileUrl);
        }
        
        [HttpPost("resize/async", Name = nameof(ResizeImageRequest))]
        public async Task<IActionResult> ResizeImageRequest([FromBody] ResizeImageRequested command)
        {   
            await _bus.PublishAsync(command);
            return Accepted();
        }
    }
}