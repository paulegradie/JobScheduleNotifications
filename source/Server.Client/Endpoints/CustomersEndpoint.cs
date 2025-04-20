using Server.Client.Base;
using Server.Contracts.Client.Endpoints.Customers;

namespace Server.Client.Endpoints;

public class CustomersEndpoint(HttpClient client) : EndpointBase(client), ICustomersEndpoint
{
    public Task<CustomersResponse> GetCustomers(GetCustomersRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}