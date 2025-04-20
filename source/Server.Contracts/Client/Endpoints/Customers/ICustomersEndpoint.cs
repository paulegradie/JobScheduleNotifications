namespace Server.Contracts.Client.Endpoints.Customers;

public interface ICustomersEndpoint
{
    Task<CustomersResponse> GetCustomersAsync(GetCustomersRequest request, CancellationToken cancellationToken);
}