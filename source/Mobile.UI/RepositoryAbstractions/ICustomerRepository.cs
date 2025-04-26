using Api.ValueTypes;
using Server.Contracts.Client.Endpoints;
using Server.Contracts.Client.Endpoints.Customers.Contracts;
using Server.Contracts.Dtos;

namespace Mobile.UI.RepositoryAbstractions;

public interface ICustomerRepository
{
    Task<OperationResult<IEnumerable<CustomerDto>>> GetCustomersAsync(CancellationToken ct = default);
    Task<OperationResult<CustomerDto>> GetCustomerByIdAsync(CustomerId id, CancellationToken ct = default);
    Task<OperationResult<CustomerDto>> CreateCustomerAsync(CreateCustomerRequest req, CancellationToken ct = default);
    Task<OperationResult<CustomerDto>> UpdateCustomerAsync(UpdateCustomerRequest req, CancellationToken ct = default);
    Task<OperationResult> DeleteCustomerAsync(CustomerId id, CancellationToken ct = default);
}