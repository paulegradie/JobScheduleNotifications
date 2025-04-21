using System.Net.Http.Json;
using System.Text.Json;
using Server.Client.Exceptions;
using Server.Contracts.Client;
using Server.Contracts.Client.Request;

namespace Server.Client.Base;

internal abstract class EndpointBase(HttpClient client) : IServerEndpoint
{
    protected readonly HttpClient Client = client;
    public async Task<TResponse> Post<TRequest, TResponse>(TRequest command, CancellationToken cancellationToken)
        where TRequest : RequestBase
    {
        var response = await Client.PostAsJsonAsync(command.GetApiRoute().ToString(), command, cancellationToken);
        await CatchErrorsAndThrow(response);
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken) ??
               throw new ResponseEmptyException(command.GetApiRoute().ToString());
    }

    public async Task<TResponse> Get<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
        where TRequest : RequestBase
    {
        var route = request.GetApiRoute().ToString();
        var response = await Client.GetAsync(route, cancellationToken);
        await CatchErrorsAndThrow(response);
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken) ??
               throw new ResponseEmptyException(request.GetApiRoute().ToString());
    }

    private static async Task CatchErrorsAndThrow(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            string raw = await response.Content.ReadAsStringAsync();

            ErrorResponse? errorResponse = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(raw) && response.Content.Headers.ContentType?.MediaType == "application/json")
                {
                    errorResponse = JsonSerializer.Deserialize<ErrorResponse>(raw);
                }
            }
            catch (JsonException)
            {
                // Ignore — we'll fall back to raw text below
            }

            var message = errorResponse?.Messages is not null && errorResponse.Messages.Any()
                ? string.Join(", ", errorResponse.Messages)
                : $"Status {(int)response.StatusCode} ({response.StatusCode}): {raw.Trim()}";

            throw new ServerClientException(message);
        }
    }

}

