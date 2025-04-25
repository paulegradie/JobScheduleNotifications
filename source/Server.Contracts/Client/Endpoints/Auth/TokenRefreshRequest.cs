using Server.Contracts.Common.Request;

namespace Server.Contracts.Client.Endpoints.Auth;

public record TokenRefreshRequest(string AccessToken, string RefreshToken) : RequestBase(Route)
{
    public const string Route = "api/auth/refresh-token";
}