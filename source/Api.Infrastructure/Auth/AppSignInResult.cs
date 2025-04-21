namespace Api.Infrastructure.Auth;

public record AppSignInResult(string Email, string AuthToken, string RefreshToken);