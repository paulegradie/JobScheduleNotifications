namespace Server.Contracts.Client.Endpoints.Home;

public interface IHomeEndpoint
{
    Task<OperationResult<HomeResponse>> PingHomeAsync(HomeRequest request, CancellationToken cancellationToken);
}