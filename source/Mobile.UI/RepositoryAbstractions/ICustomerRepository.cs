using Mobile.UI.Entities;
using Server.Contracts.Client.Endpoints;
using Server.Contracts.Client.Endpoints.Customers.Contracts;
using Server.Contracts.Dtos;

namespace Mobile.UI.RepositoryAbstractions;

public interface ICustomerRepository
{
    Task<OperationResult<IEnumerable<ServiceRecipient>>> GetCustomersAsync(CancellationToken ct = default);
    Task<OperationResult<CustomerDto>> GetCustomerByIdAsync(Guid id, CancellationToken ct = default);
    Task<OperationResult<CustomerDto>> CreateCustomerAsync(CreateCustomerRequest req, CancellationToken ct = default);
    Task<OperationResult<CustomerDto>> UpdateCustomerAsync(UpdateCustomerRequest req, CancellationToken ct = default);
    Task<OperationResult> DeleteCustomerAsync(Guid id, CancellationToken ct = default);
}