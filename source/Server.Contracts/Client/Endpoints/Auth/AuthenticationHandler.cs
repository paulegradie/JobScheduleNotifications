using System.Net;
using System.Net.Http.Headers;

namespace Server.Contracts.Client.Endpoints.Auth;

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
            // Try refresh token and retry the request once
            token = await _authEndpoint.GetValidTokenAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            response = await base.SendAsync(request, cancellationToken);
        }

        return response;
    }
}