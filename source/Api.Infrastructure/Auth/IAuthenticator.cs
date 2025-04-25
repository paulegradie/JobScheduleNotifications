using Server.Contracts.Client.Endpoints.Auth;
using Server.Contracts.Client.Endpoints.Auth.Contracts;

namespace Api.Infrastructure.Auth;

public interface IAuthenticator
{
    Task<AppSignInResult> SignIn(string email, string password, CancellationToken cancellationToken);
    Task SignOut(SignOutRequest request, CancellationToken cancellationToken);
    Task<RegistrationResult> Register(RegisterNewAdminRequest req, CancellationToken cancellationToken);
    Task<AppSignInResult> RefreshToken(string accessToken, string refreshToken, CancellationToken cancellationToken);

}