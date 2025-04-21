using Server.Contracts.Client.Request;

namespace Server.Contracts.Client.Endpoints.Home;

public record HomeRequest() : RequestBase(Route)
{
    public const string Route = "/api/home/ping";
}