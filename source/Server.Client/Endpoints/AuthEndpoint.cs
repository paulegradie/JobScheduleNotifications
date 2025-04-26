using Server.Client.Base;
using Server.Contracts.Client.Endpoints;
using Server.Contracts.Client.Endpoints.Auth;
using Server.Contracts.Client.Endpoints.Auth.Contracts;
using Server.Contracts.Common;

namespace Server.Client.Endpoints;

internal class AuthEndpoint : EndpointBase, IAuthenticationEndpoint
{
    private readonly AsyncLocal<TokenInfo?> _currentToken = new();
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    public AuthEndpoint(HttpClient client) : base(client)
    {
    }

    public Task<OperationResult<RegistrationResponse>> RegisterAsync(
        RegisterNewAdminRequest request,
        CancellationToken cancellationToken) =>

        // POST api/auth/register → no body expected
        PostAsync<RegisterNewAdminRequest, RegistrationResponse>(request, cancellationToken);

    public async Task<OperationResult<TokenInfo>> LoginAsync(
        SignInRequest request,
        CancellationToken cancellationToken)
    {
        var result = await PostAsync<SignInRequest, TokenInfo>(request, cancellationToken);
        if (result.IsSuccess)
            CurrentToken = result.Value;
        return result;
    }

    public async Task<OperationResult> LogoutAsync(
        SignOutRequest request,
        CancellationToken cancellationToken)
    {
        // clear locally first
        CurrentToken = null;
        var result = await PostAsync<SignOutRequest, Unit>(request, cancellationToken);
        return OperationResult.Success(result.StatusCode);
    }

    public async Task<OperationResult<TokenInfo>> RefreshTokenAsync(
        TokenRefreshRequest request,
        CancellationToken cancellationToken)
    {
        var result = await PostAsync<TokenRefreshRequest, TokenInfo>(request, cancellationToken);
        if (result.IsSuccess)
            CurrentToken = result.Value;
        return result;
    }

    public async Task<string?> GetValidTokenAsync()
    {
        var token = CurrentToken;
        if (token == null) return null;

        if (token.ExpiresAt <= DateTime.UtcNow.AddMinutes(5))
        {
            await _refreshLock.WaitAsync();
            try
            {
                if (CurrentToken!.ExpiresAt <= DateTime.UtcNow.AddMinutes(5))
                {
                    var refreshReq = new TokenRefreshRequest(token.AccessToken, token.RefreshToken);
                    var refreshed = await RefreshTokenAsync(refreshReq, CancellationToken.None);
                    if (refreshed.IsSuccess)
                        token = CurrentToken!;
                }
            }
            finally
            {
                _refreshLock.Release();
            }
        }

        return token.AccessToken;
    }

    public bool IsAuthenticated => CurrentToken != null;
    public void SetCurrentToken(TokenInfo tokenInfo) => CurrentToken = tokenInfo;

    private TokenInfo? CurrentToken
    {
        get => _currentToken.Value;
        set => _currentToken.Value = value;
    }
}