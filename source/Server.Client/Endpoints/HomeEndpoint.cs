using Server.Client.Base;
using Server.Contracts.Client.Endpoints.Home;

namespace Server.Client.Endpoints;

public class HomeEndpoint : EndpointBase, IHomeEndpoint
{
    public HomeEndpoint(HttpClient client) : base(client)
    {
    }

    public async Task<HomeResponse> PingHome(HomeRequest homeRequest, CancellationToken cancellationToken)
        => await Get<HomeRequest, HomeResponse>(homeRequest, cancellationToken);
}