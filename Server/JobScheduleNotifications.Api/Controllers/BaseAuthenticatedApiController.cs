using Microsoft.AspNetCore.Authorization;

namespace JobScheduleNotifications.Api.Controllers;

[Authorize]
public abstract class BaseAuthenticatedApiController : BaseApiController
{
    // Common authenticated functionality can be added here
    // For example: user context, permission checks, etc.
} 