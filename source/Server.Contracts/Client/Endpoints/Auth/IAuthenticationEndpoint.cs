namespace Server.Contracts.Client.Endpoints.Auth;

public interface IAuthenticationEndpoint
{
    Task<TokenInfo> LoginAsync(LoginRequest req);
    Task<TokenInfo> RefreshTokenAsync(string refreshToken);
    Task<bool> LogoutAsync();
    Task<bool> RegisterAsync(RegisterRequest req);
    
    // Get the current valid token or automatically refresh if needed
    Task<string> GetValidTokenAsync();

}