using System.Runtime.CompilerServices;
using Server.Client.Endpoints;
using Server.Contracts.Client;
using Server.Contracts.Client.Endpoints.Customers;
using Server.Contracts.Client.Endpoints.Home;
[assembly: InternalsVisibleTo("Mobile.Api.Composition")]

namespace Server.Client;

internal class ServerClient(HttpClient client) : IServerClient
{
    public HttpClient Http { get; set; } = client;

    public IHomeEndpoint Home { get; init; } = new HomeEndpoint(client);
    public ICustomersEndpoint Customers { get; init; } = new CustomersEndpoint(client);
}