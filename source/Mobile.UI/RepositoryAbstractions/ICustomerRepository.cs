using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Api.ValueTypes;
using Server.Contracts.Dtos;
using Server.Contracts.Endpoints;
using Server.Contracts.Endpoints.Customers.Contracts;

namespace Mobile.UI.RepositoryAbstractions;

public interface ICustomerRepository
{
    Task<OperationResult<IEnumerable<CustomerDto>>> GetCustomersAsync(CancellationToken ct = default);
    Task<OperationResult<CustomerDto>> GetCustomerByIdAsync(CustomerId id, CancellationToken ct = default);
    Task<OperationResult<CustomerDto>> CreateCustomerAsync(CreateCustomerRequest req, CancellationToken ct = default);
    Task<OperationResult<CustomerDto>> UpdateCustomerAsync(UpdateCustomerRequest req, CancellationToken ct = default);
    Task<OperationResult> DeleteCustomerAsync(CustomerId id, CancellationToken ct = default);
}