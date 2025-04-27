using Api.ValueTypes;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts;
using Server.Contracts.Dtos;
using Server.Contracts.Endpoints;
using Server.Contracts.Endpoints.Customers.Contracts;

namespace Mobile.Infrastructure.Repositories;

internal class CustomerRepository : ICustomerRepository
{
    private readonly IServerClient _server;

    public CustomerRepository(IServerClient serverClient)
        => _server = serverClient;

    public async Task<OperationResult<IEnumerable<CustomerDto>>> GetCustomersAsync(CancellationToken ct = default)
    {
        var op = await _server
            .Customers
            .GetCustomersAsync(new GetCustomersRequest(), ct);

        if (!op.IsSuccess)
            return OperationResult<IEnumerable<CustomerDto>>
                .Failure(op.ErrorMessage ?? "Unknown error", op.StatusCode);

        var customers = op.Value.Customers.ToList();

        return OperationResult<IEnumerable<CustomerDto>>
            .Success(customers, op.StatusCode);
    }

    public async Task<OperationResult<CustomerDto>> GetCustomerByIdAsync(CustomerId id, CancellationToken ct = default)
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

    public async Task<OperationResult<CustomerDto>> CreateCustomerAsync(CreateCustomerRequest req, CancellationToken ct = default)
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

    public async Task<OperationResult<CustomerDto>> UpdateCustomerAsync(UpdateCustomerRequest req, CancellationToken ct = default)
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

    public async Task<OperationResult> DeleteCustomerAsync(CustomerId id, CancellationToken ct = default)
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