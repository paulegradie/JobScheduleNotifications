namespace Api.Infrastructure.Auth.AccessPolicies;

public class AuthenticationOptions
{
    public string JwtKey { get; init; } = null!;
}