namespace Server.Contracts.Client.Endpoints.Auth;

public interface IAuthenticationEndpoint
{
    Task<TokenInfo> LoginAsync(SignInRequest req);
    Task<bool> LogoutAsync(SignOutRequest request);
    Task<bool> RegisterAsync(RegisterNewAdminRequest request);
    Task<string?> GetValidTokenAsync(); // for refresh
    Task<TokenInfo?> RefreshTokenAsync(TokenRefreshRequest refreshToken);
    Task<UserEmail> GetCurrentUserEmailAsync();
}

public record UserEmail(string Email);