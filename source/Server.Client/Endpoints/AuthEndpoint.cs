using System.Net.Http.Json;
using Server.Client.Base;
using Server.Contracts.Client.Endpoints.Auth;

namespace Server.Client.Endpoints;

internal class AuthEndpoint : EndpointBase, IAuthenticationEndpoint
{
    private TokenInfo _currentToken;
    private readonly SemaphoreSlim _refreshLock = new SemaphoreSlim(1, 1);

    public AuthEndpoint(HttpClient client) : base(client)
    {
    }

    public async Task<TokenInfo> LoginAsync(LoginRequest req)
    {
        var response = await Client.PostAsJsonAsync("auth/login", req);
        response.EnsureSuccessStatusCode();
        
        var tokenInfo = await response.Content.ReadFromJsonAsync<TokenInfo>();

        _currentToken = tokenInfo ?? throw new InvalidOperationException("Failed to parse token response");
        return tokenInfo;
    }

    public async Task<TokenInfo> RefreshTokenAsync(string refreshToken)
    {
        // Implementation for refresh token API call
        // Example:
        // var response = await Client.PostAsJsonAsync("auth/refresh", new { RefreshToken = refreshToken });
        // response.EnsureSuccessStatusCode();
        // var tokenInfo = await response.Content.ReadFromJsonAsync<TokenInfo>();

        // For now, using placeholder:
        var tokenInfo = new TokenInfo
        {
            // Populate with real values from API response
        };

        // Update the stored token info
        _currentToken = tokenInfo;
        
        return tokenInfo;
    }

    public async Task<bool> LogoutAsync()
    {
        // Implementation for logout API call
        // Example:
        // var response = await Client.PostAsync("auth/logout", null);
        // var success = response.IsSuccessStatusCode;

        // Clear the token on successful logout
        _currentToken = null;
        
        // Return success flag
        return true; // Replace with actual API call result
    }

    public Task<bool> RegisterAsync(RegisterRequest req)
    {
        // Implementation for registration API call
        // Example:
        // var response = await Client.PostAsJsonAsync("auth/register", req);
        // return response.IsSuccessStatusCode;
        
        return Task.FromResult(true); // Replace with actual API call
    }

    public async Task<string> GetValidTokenAsync()
    {
        // If we don't have a token yet, return null or throw as appropriate
        if (_currentToken == null)
        {
            return null;
        }

        // Check if the current token is close to expiring (e.g., within 5 minutes)
        if (_currentToken.ExpiresAt <= DateTime.UtcNow.AddMinutes(5))
        {
            // Use a semaphore to prevent multiple simultaneous refresh attempts
            await _refreshLock.WaitAsync();
            try
            {
                // Double-check that the token still needs refreshing after acquiring the lock
                if (_currentToken.ExpiresAt <= DateTime.UtcNow.AddMinutes(5))
                {
                    // Refresh the token
                    _currentToken = await RefreshTokenAsync(_currentToken.RefreshToken);
                }
            }
            finally
            {
                _refreshLock.Release();
            }
        }

        return _currentToken.AccessToken;
    }
    
    public bool IsAuthenticated => _currentToken != null && _currentToken.ExpiresAt > DateTime.UtcNow;
}