using Mobile.UI.RepositoryAbstractions;
using Mobile.UI.Services;
using Server.Contracts.Dtos;

namespace Mobile.Infrastructure.Services;

internal class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<CustomerDto>> GetCustomersAsync()
    {
        var result = await _customerRepository.GetCustomersAsync();
        if (result.IsSuccess)
            return result.Value;

        throw new Exception(result.ErrorMessage ?? "Failed to load customers.");
    }
}