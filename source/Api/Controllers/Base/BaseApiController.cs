using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Base;

[ApiController] 
public abstract class BaseApiController : ControllerBase
{
    // Common functionality can be added here
    // For example: logging, common error handling, etc.
}