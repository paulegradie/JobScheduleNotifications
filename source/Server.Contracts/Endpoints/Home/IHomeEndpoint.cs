using Server.Contracts.Endpoints.Home.Contracts;

namespace Server.Contracts.Endpoints.Home;

public interface IHomeEndpoint
{
    Task<OperationResult<HomeResponse>> PingHomeAsync(HomeRequest request, CancellationToken cancellationToken);
}