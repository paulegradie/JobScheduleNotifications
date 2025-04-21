using System.Runtime.CompilerServices;
using Server.Client.Endpoints;
using Server.Contracts.Client;
using Server.Contracts.Client.Endpoints.Auth;
using Server.Contracts.Client.Endpoints.Customers;
using Server.Contracts.Client.Endpoints.Home;

[assembly: InternalsVisibleTo("Mobile.Api.Composition")]

namespace Server.Client;

internal class ServerClient : IServerClient
{
    public ServerClient(HttpClient client, IAuthenticationEndpoint authEndpoint)
    {
        var handler = new AuthenticationHandler(authEndpoint);
        Http = new HttpClient(handler) 
        {
            BaseAddress = client.BaseAddress
        };

        Home = new HomeEndpoint(Http);
        Customers = new CustomersEndpoint(Http);
        Auth = authEndpoint;
    }



    public HttpClient Http { get; set; }

    public IHomeEndpoint Home { get; init; }
    public ICustomersEndpoint Customers { get; init; }

    public IAuthenticationEndpoint Auth { get; init; }
}