using Server.Contracts.Client.Endpoints.Home;

namespace Server.Contracts.Client.Endpoints.Customers;

public interface ICustomersEndpoint
{
    Task<CustomersResponse> GetCustomers(GetCustomersRequest request, CancellationToken cancellationToken);
}