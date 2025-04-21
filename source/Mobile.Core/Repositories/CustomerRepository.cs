using Mobile.Core.Domain;
using Server.Contracts.Client;
using Server.Contracts.Client.Endpoints.Customers;
using Server.Contracts.Customers;

namespace Mobile.Core.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IServerClient _serverClient;

    public CustomerRepository(IServerClient serverClient)
    {
        _serverClient = serverClient;
    }

    public async Task<IEnumerable<ServiceRecipient>> GetServiceRecipients()
    {
        var result = await _serverClient.Customers.GetCustomersAsync(new GetCustomersRequest(), CancellationToken.None);
        Console.WriteLine("IT WORKED");
        var entities = result.Customers.Select(x => new ServiceRecipient(x));
        return entities;
    }

    public async Task<CustomerDto> GetCustomerByIdAsync(Guid id)
    {
        return await _serverClient.Customers.GetCustomerByIdAsync(id, CancellationToken.None)
               ?? throw new InvalidOperationException($"Customer with ID {id} not found");
    }

    public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto customer)
    {
        return await _serverClient.Customers.CreateCustomerAsync(customer, CancellationToken.None)
               ?? throw new InvalidOperationException("Failed to create customer");
    }

    public async Task<CustomerDto> UpdateCustomerAsync(Guid id, UpdateCustomerDto customer)
    {
        return await _serverClient.Customers.UpdateCustomerAsync(id, customer, CancellationToken.None)
               ?? throw new InvalidOperationException($"Failed to update customer with ID {id}");
    }

    public async Task DeleteCustomerAsync(Guid id)
    {
        await _serverClient.Customers.DeleteCustomerAsync(id, CancellationToken.None);
    }
}