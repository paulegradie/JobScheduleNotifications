using Server.Contracts.Client.Endpoints;
using Server.Contracts.Client.Request;

namespace Server.Contracts.Client;

public interface IServerEndpoint
{
    Task<TResponse> Post<TRequest, TResponse>(TRequest command, CancellationToken cancellationToken)
        where TRequest : RequestBase;

    Task<TResponse> Get<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
        where TRequest : RequestBase;
}