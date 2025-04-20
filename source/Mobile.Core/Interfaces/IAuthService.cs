using Mobile.Core.Models;

namespace Mobile.Core.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);
    Task LoginAsync(LoginRequest request);
    /// <summary>Get the stored JWT (or null if none).</summary>
    string? GetToken();
    /// <summary>Clears any stored token.</summary>
    Task LogoutAsync();
}