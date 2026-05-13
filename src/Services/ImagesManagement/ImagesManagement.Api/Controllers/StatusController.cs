using Microsoft.AspNetCore.Mvc;

namespace ImagesManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StatusController : ControllerBase
{
    [HttpGet(Name = nameof(GetStatus))]
    public Task<string> GetStatus()
    {
        return Task.FromResult("OK");
    }
}