using System.Net.Http.Json;
using Server.Client.Base;
using Server.Contracts.Client.Endpoints.Auth;

namespace Server.Client.Endpoints;

internal class AuthEndpoint : EndpointBase, IAuthenticationEndpoint
{
    private readonly AsyncLocal<TokenInfo?> _currentToken = new();
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    public AuthEndpoint(HttpClient client) : base(client)
    {
    }

    public async Task<UserEmail> GetCurrentUserEmailAsync()
    {
        await Task.CompletedTask;
        if (CurrentToken is null) throw new InvalidOperationException("Not authenticated");
        return new UserEmail(CurrentToken.Email);
    }

    public async Task<bool> RegisterAsync(RegisterNewAdminRequest req)
    {
        try
        {
            var response = await Client.PostAsJsonAsync(req.ApiRoute, req);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<TokenInfo> LoginAsync(SignInRequest req)
    {
        var response = await Client.PostAsJsonAsync(req.ApiRoute, req);
        response.EnsureSuccessStatusCode();

        var tokenInfo = await response.Content.ReadFromJsonAsync<TokenInfo>();

        CurrentToken = tokenInfo ?? throw new InvalidOperationException("Failed to parse token response");
        return tokenInfo;
    }

    public async Task<bool> LogoutAsync(SignOutRequest request)
    {
        try
        {
            // Clear the token regardless of server response
            var currentToken = CurrentToken;
            CurrentToken = null;

            // Only notify server if we had a token to invalidate
            if (currentToken != null)
            {
                // Optional: Send the refresh token to be invalidated
                var response = await Client.PostAsync(SignOutRequest.Route, null);
                return response.IsSuccessStatusCode;
            }

            return true;
        }
        catch
        {
            // We've already cleared the token locally, so the user is effectively logged out
            return true;
        }
    }

    public async Task<TokenInfo?> RefreshTokenAsync(TokenRefreshRequest request)
    {
        var response = await Client.PostAsJsonAsync(request.ApiRoute, new { request.RefreshToken });
        response.EnsureSuccessStatusCode();

        var tokenInfo = await response.Content.ReadFromJsonAsync<TokenInfo>();

        CurrentToken = tokenInfo ?? throw new InvalidOperationException("Failed to parse refresh token response");
        return tokenInfo;
    }

    public async Task<string?> GetValidTokenAsync()
    {
        // If we don't have a token yet, return null or throw as appropriate
        if (CurrentToken == null)
        {
            return null;
        }

        // Check if the current token is close to expiring (e.g., within 5 minutes)
        if (CurrentToken.ExpiresAt <= DateTime.UtcNow.AddMinutes(5))
        {
            await _refreshLock.WaitAsync();
            try
            {
                // Double-check that the token still needs refreshing after acquiring the lock
                if (CurrentToken.ExpiresAt <= DateTime.UtcNow.AddMinutes(5))
                {
                    CurrentToken = await RefreshTokenAsync(new TokenRefreshRequest(CurrentToken.AccessToken, CurrentToken.RefreshToken));
                }
            }
            finally
            {
                _refreshLock.Release();
            }
        }

        return CurrentToken?.AccessToken;
    }

    public bool IsAuthenticated => CurrentToken != null;

    private TokenInfo? CurrentToken
    {
        get => _currentToken.Value;
        set => _currentToken.Value = value;
    }
}