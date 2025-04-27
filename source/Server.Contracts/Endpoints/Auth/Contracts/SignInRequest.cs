using Server.Contracts.Common.Request;

namespace Server.Contracts.Endpoints.Auth.Contracts;

public record SignInRequest(string Email, string Password) : RequestBase(Route)
{
    public const string Route = "/api/auth/sign-in";
}