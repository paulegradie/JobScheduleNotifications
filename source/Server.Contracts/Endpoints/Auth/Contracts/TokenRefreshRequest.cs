using Server.Contracts.Common.Request;

namespace Server.Contracts.Endpoints.Auth.Contracts;

public record TokenRefreshRequest(string AccessToken, string RefreshToken) : RequestBase(Route)
{
    public const string Route = "api/auth/refresh-token";
}