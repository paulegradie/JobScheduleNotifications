using JobScheduleNotifications.Contracts.Customers;

namespace JobScheduleNotifications.Mobile.Services;

public class CustomerService : ICustomerService
{
    private readonly HttpClientService _httpClient;
    private const string BaseEndpoint = "customers";

    public CustomerService(HttpClientService httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<CustomerDto>> GetCustomersAsync()
    {
        return await _httpClient.GetListAsync<CustomerDto>(BaseEndpoint) ?? Enumerable.Empty<CustomerDto>();
    }

    public async Task<CustomerDto> GetCustomerByIdAsync(Guid id)
    {
        return await _httpClient.GetAsync<CustomerDto>($"{BaseEndpoint}/{id}") 
            ?? throw new InvalidOperationException($"Customer with ID {id} not found");
    }

    public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto customer)
    {
        return await _httpClient.PostAsync<CustomerDto>(BaseEndpoint, customer) 
            ?? throw new InvalidOperationException("Failed to create customer");
    }

    public async Task<CustomerDto> UpdateCustomerAsync(Guid id, UpdateCustomerDto customer)
    {
        return await _httpClient.PutAsync<CustomerDto>($"{BaseEndpoint}/{id}", customer) 
            ?? throw new InvalidOperationException($"Failed to update customer with ID {id}");
    }

    public async Task DeleteCustomerAsync(Guid id)
    {
        await _httpClient.DeleteAsync($"{BaseEndpoint}/{id}");
    }
} 