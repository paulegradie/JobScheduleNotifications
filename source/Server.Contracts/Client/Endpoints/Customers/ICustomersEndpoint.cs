using Server.Contracts.Client.Endpoints.Customers.Contracts;

namespace Server.Contracts.Client.Endpoints.Customers;

public interface ICustomersEndpoint
{
    Task<OperationResult<GetCustomersResponse>> GetCustomersAsync(GetCustomersRequest request, CancellationToken ct);
    Task<OperationResult<GetCustomerByIdResponse>> GetCustomerByIdAsync(GetCustomerByIdRequest request, CancellationToken ct);
    Task<OperationResult<CreateCustomerResponse>> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken ct);
    Task<OperationResult<UpdateCustomerResponse>> UpdateCustomerAsync(UpdateCustomerRequest request, CancellationToken ct);
    Task<OperationResult<DeleteCustomerResponse>> DeleteCustomerAsync(DeleteCustomerRequest request, CancellationToken ct);
}