using System.Net.Http.Json;
using Server.Client.Exceptions;
using Server.Contracts.Client;
using Server.Contracts.Client.Request;

namespace Server.Client.Base;

public abstract class EndpointBase(HttpClient client) : IServerEndpoint
{
    public async Task<TResponse> Post<TRequest, TResponse>(TRequest command, CancellationToken cancellationToken)
        where TRequest : RequestBase
    {
        var response = await client.PostAsJsonAsync(command.GetActionRoute().ToString(), command, cancellationToken);
        await CatchErrorsAndThrow(response);
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken) ??
               throw new ResponseEmptyException(command.GetActionRoute().ToString());
    }

    public async Task<TResponse> Get<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
        where TRequest : RequestBase
    {
        var response = await client.GetAsync(request.GetActionRoute().ToString(), cancellationToken);
        await CatchErrorsAndThrow(response);
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken) ??
               throw new ResponseEmptyException(request.GetActionRoute().ToString());
    }

    private static async Task CatchErrorsAndThrow(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new ServerClientException(string.Join(", ", errorResponse?.Messages ?? []));
        }
    }
}

