using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Server.Contracts.Client.Endpoints.Auth;

namespace Server.Client.Base;

public class AuthenticationHandler : DelegatingHandler
{
    private readonly IAuthenticationEndpoint _authEndpoint;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public AuthenticationHandler(IAuthenticationEndpoint authEndpoint)
    {
        _authEndpoint = authEndpoint;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Add token to request
        var token = await _authEndpoint.GetValidTokenAsync();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await base.SendAsync(request, cancellationToken);

        // If unauthorized, token might be expired
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            token = await _authEndpoint.GetValidTokenAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            response = await base.SendAsync(request, cancellationToken);
        }

        return response;
    }
}