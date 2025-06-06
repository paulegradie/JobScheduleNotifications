using Server.Client.Base;
using Server.Contracts.Endpoints;
using Server.Contracts.Endpoints.Home;
using Server.Contracts.Endpoints.Home.Contracts;

namespace Server.Client.Endpoints;

internal class HomeEndpoint : EndpointBase, IHomeEndpoint
{
    public HomeEndpoint(HttpClient client) : base(client) { }

    public Task<OperationResult<HomeResponse>> PingHomeAsync(
        HomeRequest request,
        CancellationToken cancellationToken) =>
        
        // GET api/home → HomeResponse wrapped in OperationResult
        GetAsync<HomeRequest, HomeResponse>(request, cancellationToken);
}