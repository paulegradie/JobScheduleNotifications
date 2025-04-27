using Server.Contracts.Common.Request;

namespace Server.Contracts.Endpoints.Home.Contracts;

public record HomeRequest() : RequestBase(Route)
{
    public const string Route = "/api/home/ping";
}