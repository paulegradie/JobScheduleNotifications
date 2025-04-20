namespace Server.Contracts.Client.Endpoints.Home;

public interface IHomeEndpoint : IServerEndpoint
{
    Task<HomeResponse> PingHome(HomeRequest homeRequest, CancellationToken cancellationToken);
}