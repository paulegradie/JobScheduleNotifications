using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers.Base;

[Authorize]
public abstract class BaseAuthenticatedApiController : BaseApiController
{
    // Common authenticated functionality can be added here
    // For example: user context, permission checks, etc.
} 