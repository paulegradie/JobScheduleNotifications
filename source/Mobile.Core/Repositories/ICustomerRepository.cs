using Mobile.Core.Domain;
using Server.Contracts.Customers;

namespace Mobile.Core.Repositories;

public interface ICustomerRepository
{
    Task<IEnumerable<ServiceRecipient>> GetServiceRecipients();
    Task<CustomerDto> GetCustomerByIdAsync(Guid id);
    Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto customer);
    Task<CustomerDto> UpdateCustomerAsync(Guid id, UpdateCustomerDto customer);
    Task DeleteCustomerAsync(Guid id);
} 