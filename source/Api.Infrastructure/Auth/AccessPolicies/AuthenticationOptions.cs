namespace Api.Infrastructure.Auth.AccessPolicies;

public class AuthenticationOptions
{
    public const string  Node = "Jwt";
    public string Key { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
}