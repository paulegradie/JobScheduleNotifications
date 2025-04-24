using Mobile.Core.Domain;
using Server.Contracts.Client;
using Server.Contracts.Client.Endpoints;
using Server.Contracts.Client.Endpoints.Customers.Contracts;

namespace Mobile.Core.Repositories;

public class CustomerRepository : ICustomerRepository
{
    readonly IServerClient _server;

    public CustomerRepository(IServerClient serverClient)
        => _server = serverClient;

    public async Task<OperationResult<IEnumerable<ServiceRecipient>>> 
        GetCustomersAsync(CancellationToken ct = default)
    {
        var op = await _server
            .Customers
            .GetCustomersAsync(new GetCustomersRequest(), ct);

        if (!op.IsSuccess)
            return OperationResult<IEnumerable<ServiceRecipient>>
                .Failure(op.ErrorMessage ?? "Unknown error", op.StatusCode);

        var customers = op.Value.Customers
            .Select(dto => new ServiceRecipient(dto))
            .ToList();

        return OperationResult<IEnumerable<ServiceRecipient>>
            .Success(customers, op.StatusCode);
    }

    public async Task<OperationResult<CustomerDto>> 
        GetCustomerByIdAsync(Guid id, CancellationToken ct = default)
    {
        var op = await _server
            .Customers
            .GetCustomerByIdAsync(new GetCustomerByIdRequest(id), ct);

        if (!op.IsSuccess)
            return OperationResult<CustomerDto>
                .Failure(op.ErrorMessage ?? $"Couldn’t load {id}", op.StatusCode);

        return OperationResult<CustomerDto>
            .Success(op.Value.Customer, op.StatusCode);
    }

    public async Task<OperationResult<CustomerDto>> 
        CreateCustomerAsync(CreateCustomerRequest req, CancellationToken ct = default)
    {
        var op = await _server
            .Customers
            .CreateCustomerAsync(req, ct);

        if (!op.IsSuccess)
            return OperationResult<CustomerDto>
                .Failure(op.ErrorMessage ?? "Create failed", op.StatusCode);

        return OperationResult<CustomerDto>
            .Success(op.Value.Customer, op.StatusCode);
    }

    public async Task<OperationResult<CustomerDto>> 
        UpdateCustomerAsync(UpdateCustomerRequest req, CancellationToken ct = default)
    {
        var op = await _server
            .Customers
            .UpdateCustomerAsync(req, ct);

        if (!op.IsSuccess)
            return OperationResult<CustomerDto>
                .Failure(op.ErrorMessage ?? $"Update failed for {req.Id}", op.StatusCode);

        return OperationResult<CustomerDto>
            .Success(op.Value.Customer, op.StatusCode);
    }

    public async Task<OperationResult> 
        DeleteCustomerAsync(Guid id, CancellationToken ct = default)
    {
        var op = await _server
            .Customers
            .DeleteCustomerAsync(new DeleteCustomerRequest(id), ct);

        if (!op.IsSuccess)
            return OperationResult
                .Failure(op.ErrorMessage ?? $"Delete failed for {id}", op.StatusCode);

        return OperationResult.Success(op.StatusCode);
    }
}