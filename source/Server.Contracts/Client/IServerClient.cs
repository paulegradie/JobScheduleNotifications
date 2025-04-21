using Server.Contracts.Client.Endpoints.Auth;
using Server.Contracts.Client.Endpoints.Customers;
using Server.Contracts.Client.Endpoints.Home;

namespace Server.Contracts.Client;

public interface IServerClient
{
    public IHomeEndpoint Home { get; init; }
    public ICustomersEndpoint Customers { get; init; }
    public IAuthenticationEndpoint Auth { get; init; }
}