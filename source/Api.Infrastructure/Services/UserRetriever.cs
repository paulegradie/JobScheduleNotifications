using System.Security.Claims;
using Api.Infrastructure.DbTables.OrganizationModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Api.Infrastructure.Services;

public class UserRetriever : IUserRetriever
{
    private readonly UserManager<ApplicationUserRecord> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserRetriever(UserManager<ApplicationUserRecord> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApplicationUserRecord> GetCurrentUserAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            throw new UserNotFoundException("User ID not found in token");

        var user = await _userManager.FindByIdAsync(userId);
        return user ?? throw new UserNotFoundException("User not found in database");
    }
}

internal class UserNotFoundException(string? message) : Exception(message);