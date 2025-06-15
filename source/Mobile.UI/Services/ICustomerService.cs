using Api.ValueTypes;
using Server.Contracts.Dtos;

namespace Mobile.UI.Services;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetCustomersAsync();
    Task<CustomerDto> GetCustomerAsync(CustomerId customerId);
}