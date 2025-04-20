using Mobile.Core.Interfaces;
using Server.Contracts.Customers;

namespace Mobile.Infrastructure.Services;

public sealed class CustomerService : ICustomerService
{
    private const string Endpoint = "customers";
    private readonly IApiClient _api;

    public CustomerService(IApiClient api) => _api = api;

    public async Task<IEnumerable<CustomerDto>> GetCustomersAsync() =>
        await _api.GetListAsync<CustomerDto>(Endpoint);

    public async Task<CustomerDto> GetCustomerByIdAsync(Guid id) =>
        await _api.GetAsync<CustomerDto>($"{Endpoint}/{id}")
            .ContinueWith(t => t.Result
                               ?? throw new InvalidOperationException($"Customer {id} not found"));

    public Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto c) =>
        _api.PostAsync<CustomerDto>(Endpoint, c)
            .ContinueWith(t => t.Result
                               ?? throw new InvalidOperationException("Creation failed"));

    public Task<CustomerDto> UpdateCustomerAsync(Guid id, UpdateCustomerDto c) =>
        _api.PutAsync<CustomerDto>($"{Endpoint}/{id}", c)
            .ContinueWith(t => t.Result
                               ?? throw new InvalidOperationException("Update failed"));

    public Task DeleteCustomerAsync(Guid id) =>
        _api.DeleteAsync($"{Endpoint}/{id}");
}