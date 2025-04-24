using Server.Client.Base;
using Server.Contracts.Client.Endpoints;
using Server.Contracts.Client.Endpoints.Customers;
using Server.Contracts.Client.Endpoints.Customers.Contracts;

internal class CustomersEndpoint : EndpointBase, ICustomersEndpoint
{
    public CustomersEndpoint(HttpClient client) : base(client) { }

    public Task<OperationResult<GetCustomersResponse>> GetCustomersAsync(GetCustomersRequest request, CancellationToken ct) =>
        GetAsync<GetCustomersRequest, GetCustomersResponse>(request, ct);

    public Task<OperationResult<GetCustomerByIdResponse>> GetCustomerByIdAsync(GetCustomerByIdRequest request, CancellationToken ct) =>
        GetAsync<GetCustomerByIdRequest, GetCustomerByIdResponse>(request, ct);

    public Task<OperationResult<CreateCustomerResponse>> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken ct) =>
        PostAsync<CreateCustomerRequest, CreateCustomerResponse>(request, ct);

    public Task<OperationResult<UpdateCustomerResponse>> UpdateCustomerAsync(UpdateCustomerRequest request, CancellationToken ct) =>
        PutAsync<UpdateCustomerRequest, UpdateCustomerResponse>(request, ct);

    public Task<OperationResult<DeleteCustomerResponse>> DeleteCustomerAsync(DeleteCustomerRequest request, CancellationToken ct) =>
        DeleteAsync<DeleteCustomerRequest, DeleteCustomerResponse>(request, ct);
}