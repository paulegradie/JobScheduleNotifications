using System.Runtime.CompilerServices;
using Server.Client.Endpoints;
using Server.Contracts.Client;
using Server.Contracts.Client.Endpoints.Auth;
using Server.Contracts.Client.Endpoints.Customers;
using Server.Contracts.Client.Endpoints.Home;

[assembly: InternalsVisibleTo("Mobile.Api.Composition")]
[assembly: InternalsVisibleTo("IntegrationTests")]

namespace Server.Client;

internal class ServerClient : IServerClient
{
    public ServerClient(HttpClient client)
    {
        Http = client;
        Home = new HomeEndpoint(client);
        Customers = new CustomersEndpoint(client);
        Auth = new AuthEndpoint(client);
    }


    public HttpClient Http { get; set; }

    public IHomeEndpoint Home { get; init; }
    public ICustomersEndpoint Customers { get; init; }
    public IAuthenticationEndpoint Auth { get; init; }
}