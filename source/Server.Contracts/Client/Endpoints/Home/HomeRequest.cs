using Server.Contracts.Client.Request;

namespace Server.Contracts.Client.Endpoints.Home;

public record HomeRequest() : RequestBase("api/home")
{
}