using JobScheduleNotifications.Contracts.Authentication;

namespace Mobile.Services;

public interface IAuthService
{
    Task<bool> LoginAsync(string email, string password);
    Task<bool> RegisterAsync(RegisterBusinessOwnerDto registration);
    Task<bool> LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<string> GetAuthTokenAsync();
} 