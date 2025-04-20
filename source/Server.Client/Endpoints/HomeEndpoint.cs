using Server.Client.Base;
using Server.Contracts.Client.Endpoints.Home;

namespace Server.Client.Endpoints;

internal class HomeEndpoint : EndpointBase, IHomeEndpoint
{
    public HomeEndpoint(HttpClient client) : base(client)
    {
    }

    public async Task<HomeResponse> PingHome(HomeRequest homeRequest, CancellationToken cancellationToken)
    {
        return await Get<HomeRequest, HomeResponse>(homeRequest, cancellationToken);
    }
}