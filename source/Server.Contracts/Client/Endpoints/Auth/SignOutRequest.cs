using Server.Contracts.Client.Request;

namespace Server.Contracts.Client.Endpoints.Auth;

public record SignOutRequest(string Email) : RequestBase(Route)
{
    public const string Route = "api/auth/sign-out";
};