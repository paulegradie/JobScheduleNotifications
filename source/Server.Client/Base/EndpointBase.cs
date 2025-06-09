using System.Net.Http.Json;
using System.Text.Json;
using Server.Client.Exceptions;
using Server.Contracts.Common;
using Server.Contracts.Common.Request;
using Server.Contracts.Endpoints;

namespace Server.Client.Base;

internal abstract class EndpointBase
{
    protected readonly HttpClient Client;

    protected EndpointBase(HttpClient client) => Client = client;

    public Task<OperationResult<TResponse>> GetAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken ct)
        where TRequest : RequestBase
        => SendAsync<TRequest, TResponse>(HttpMethod.Get, request, ct);

    public Task<OperationResult<TResponse>> PostAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken ct)
        where TRequest : RequestBase
        => SendAsync<TRequest, TResponse>(HttpMethod.Post, request, ct);

    public Task<OperationResult<TResponse>> PutAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken ct)
        where TRequest : RequestBase
        => SendAsync<TRequest, TResponse>(HttpMethod.Put, request, ct);

    public Task<OperationResult<TResponse>> DeleteAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken ct)
        where TRequest : RequestBase
        => SendAsync<TRequest, TResponse>(HttpMethod.Delete, request, ct);

    private async Task<OperationResult<TResponse>> SendAsync<TRequest, TResponse>(
        HttpMethod method,
        TRequest request,
        CancellationToken ct)
        where TRequest : RequestBase
    {
        // 1. Build URL
        var route = request.ApiRoute;

        // 2. Pick correct HttpRequestMessage
        using var message = new HttpRequestMessage(method, route);
        if (method == HttpMethod.Post || method == HttpMethod.Put)
            message.Content = JsonContent.Create(request);

        // 3. Send
        
        HttpResponseMessage response;
        try
        {

            response = await Client.SendAsync(message, ct);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            ;
            throw;
        }

        // 4. Read body
        var raw = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            // try to deserialize ErrorResponse
            string errorMsg;
            try
            {
                var err = JsonSerializer.Deserialize<Exceptions.ErrorResponse>(raw);
                errorMsg = err?.Messages?.Any() == true
                    ? string.Join(", ", err!.Messages!)
                    : raw.Trim();
            }
            catch (JsonException)
            {
                errorMsg = raw.Trim();
            }

            return OperationResult<TResponse>.Failure(
                errorMsg,
                response.StatusCode
            );
        }

        // 5. On success, try to read out TResponse
        if (string.IsNullOrEmpty(raw))
        {
            return OperationResult<TResponse>.Success(
                default,
                response.StatusCode
            );
        }
        
        TResponse? value = default;
        if (typeof(TResponse) != typeof(Unit)) // Unit signals “no body expected”
        {
            try
            {
                value = JsonSerializer.Deserialize<TResponse>(raw, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    IncludeFields = true
                });
            }
            catch (JsonException ex)
            {
                throw new ResponseDeserializationException(
                    $"Failed to deserialize response from {route}", ex);
            }

            if (value == null)
                throw new ResponseEmptyException(route);
        }

        return OperationResult<TResponse>.Success(
            value!,
            response.StatusCode
        );
    }
}

