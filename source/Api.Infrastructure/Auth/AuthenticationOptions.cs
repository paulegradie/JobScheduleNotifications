namespace Api.Infrastructure.Auth;

public class AuthenticationOptions
{
    public const string Node = "Authentication";
    public string Key { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
}