using Server.Contracts.Dtos;

namespace Mobile.UI.Services;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetCustomersAsync();
}