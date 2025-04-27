using System.Net;
using System.Net.Http.Headers;
using Server.Contracts;

namespace Server.Client.Base;

public class AuthenticationHandler : DelegatingHandler
{
    private readonly IAuthClient _authClient;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public AuthenticationHandler(IAuthClient authClient)
    {
        _authClient = authClient;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _authClient.Auth.GetValidTokenAsync();
        // only set the header if we have one
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);

        // if 401, kick off a refresh and retry once
        if (response.StatusCode != HttpStatusCode.Unauthorized) return response;

        token = await _authClient.Auth.GetValidTokenAsync();

        if (string.IsNullOrEmpty(token)) return response;

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        response = await base.SendAsync(request, cancellationToken);

        return response;
    }
}