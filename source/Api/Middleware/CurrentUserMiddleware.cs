using System.Security.Claims;
using Api.Infrastructure.Services;

namespace Api.Middleware;

/// <summary>
/// Middleware to populate the CurrentUserService from JWT claims.
/// </summary>
public class CurrentUserMiddleware
{
    private readonly RequestDelegate _next;

    public CurrentUserMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUserService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userIdStr = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdStr, out var uid))
            {
                currentUserService.UserId = uid;
            }

            var custIdStr = context.User.FindFirstValue("customer_id");
            if (Guid.TryParse(custIdStr, out var cid))
                currentUserService.CustomerId = cid;
        }

        await _next(context);
    }
}