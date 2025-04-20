using Server.Contracts.Authentication;

namespace Mobile.Core.Services;

public interface IAuthService
{
    Task<bool> LoginAsync(string email, string password);
    Task<bool> RegisterAsync(RegisterBusinessOwnerDto registration);
    Task<bool> LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<string> GetAuthTokenAsync();
} 