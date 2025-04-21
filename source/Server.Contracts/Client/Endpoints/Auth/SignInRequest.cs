using Server.Contracts.Client.Request;

namespace Server.Contracts.Client.Endpoints.Auth;

public record SignInRequest(string Email, string Password) : RequestBase(Route)
{
    public const string Route = "/api/auth/sign-in";
}