using Microsoft.AspNetCore.Mvc;

namespace JobScheduleNotifications.Api.Controllers;

[ApiController]
[Route("api")]
public abstract class BaseApiController : ControllerBase
{
    // Common functionality can be added here
    // For example: logging, common error handling, etc.
}