using Server.Contracts.Customers;

namespace Mobile.Core.Services;

public class CustomerService : ICustomerService
{
    private readonly HttpClientService _httpClient;
    private const string baseEndpoint = "customers";

    public CustomerService(HttpClientService httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<CustomerDto>> GetCustomersAsync()
    {
        return await _httpClient.GetListAsync<CustomerDto>(baseEndpoint) ?? [];
    }

    public async Task<CustomerDto> GetCustomerByIdAsync(Guid id)
    {
        return await _httpClient.GetAsync<CustomerDto>($"{baseEndpoint}/{id}") 
            ?? throw new InvalidOperationException($"Customer with ID {id} not found");
    }

    public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto customer)
    {
        return await _httpClient.PostAsync<CustomerDto>(baseEndpoint, customer) 
            ?? throw new InvalidOperationException("Failed to create customer");
    }

    public async Task<CustomerDto> UpdateCustomerAsync(Guid id, UpdateCustomerDto customer)
    {
        return await _httpClient.PutAsync<CustomerDto>($"{baseEndpoint}/{id}", customer) 
            ?? throw new InvalidOperationException($"Failed to update customer with ID {id}");
    }

    public async Task DeleteCustomerAsync(Guid id)
    {
        await _httpClient.DeleteAsync($"{baseEndpoint}/{id}");
    }
} 