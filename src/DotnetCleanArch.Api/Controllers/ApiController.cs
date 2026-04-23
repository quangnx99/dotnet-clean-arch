using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace DotnetCleanArch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiController : ControllerBase
{
    protected readonly ISender Sender;

    protected ApiController(ISender sender) => Sender = sender;
}
