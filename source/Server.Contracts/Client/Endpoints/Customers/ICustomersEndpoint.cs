using Server.Contracts.Customers;

namespace Server.Contracts.Client.Endpoints.Customers;

public interface ICustomersEndpoint
{
    Task<CustomersResponse> GetCustomersAsync(GetCustomersRequest request, CancellationToken cancellationToken);
    Task<CustomerDto?> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<CustomerDto?> CreateCustomerAsync(CreateCustomerDto customer, CancellationToken cancellationToken);
    Task<CustomerDto?> UpdateCustomerAsync(Guid id, UpdateCustomerDto customer, CancellationToken cancellationToken);
    Task DeleteCustomerAsync(Guid id, CancellationToken cancellationToken);
}