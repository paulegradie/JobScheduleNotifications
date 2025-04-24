using Server.Client.Base;
using Server.Contracts.Client.Endpoints;
using Server.Contracts.Client.Endpoints.Home;

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