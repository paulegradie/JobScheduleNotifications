using Server.Contracts.Client.Endpoints.Auth.Contracts;

namespace Server.Contracts.Client.Endpoints.Auth;

public interface IAuthenticationEndpoint
{
    Task<OperationResult<RegistrationResponse>> RegisterAsync(RegisterNewAdminRequest request, CancellationToken cancellationToken);
    Task<OperationResult<TokenInfo>> LoginAsync(SignInRequest request, CancellationToken cancellationToken);
    Task<OperationResult> LogoutAsync(SignOutRequest request, CancellationToken cancellationToken);
    Task<OperationResult<TokenInfo>> RefreshTokenAsync(TokenRefreshRequest request, CancellationToken cancellationToken);

    // these remain “helper” APIs:
    Task<string?> GetValidTokenAsync();
    bool IsAuthenticated { get; }
}

public record UserEmail(string Email);