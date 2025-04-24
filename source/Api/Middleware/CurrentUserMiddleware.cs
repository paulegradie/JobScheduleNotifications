using System.Security.Claims;
using Api.Infrastructure.Services;

public class CurrentUserMiddleware
{
    private readonly RequestDelegate _next;

    public CurrentUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUserService)
    {
        Console.WriteLine("User.Identity.IsAuthenticated: " + context.User.Identity?.IsAuthenticated);
        Console.WriteLine("All Claims:");
        foreach (var claim in context.User.Claims)
        {
            Console.WriteLine($" - {claim.Type}: {claim.Value}");
        }
        
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userIdStr = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdStr, out var userId))
            {
                currentUserService.UserId = userId;
            }
        }

        await _next(context);
    }
}