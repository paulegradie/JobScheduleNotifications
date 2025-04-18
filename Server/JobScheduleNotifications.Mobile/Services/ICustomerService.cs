using JobScheduleNotifications.Contracts.Customers;

namespace JobScheduleNotifications.Mobile.Services;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetCustomersAsync();
    Task<CustomerDto> GetCustomerByIdAsync(Guid id);
    Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto customer);
    Task<CustomerDto> UpdateCustomerAsync(Guid id, UpdateCustomerDto customer);
    Task DeleteCustomerAsync(Guid id);
} 