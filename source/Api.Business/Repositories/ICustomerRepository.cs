using Api.Business.Entities;
using Api.ValueTypes;

namespace Api.Business.Repositories;

public interface ICustomerRepository
{
    /// <summary>
    /// Gets a customer by their ID
    /// </summary>
    Task<CustomerDomainModel?> GetByIdAsync(CustomerId customerId);
    
    /// <summary>
    /// Gets all customers for an organization
    /// </summary>
    Task<IEnumerable<CustomerDomainModel>> GetByOrganizationAsync(OrganizationId organizationId);
    
    /// <summary>
    /// Creates a new customer
    /// </summary>
    Task<CustomerDomainModel> CreateAsync(CustomerDomainModel customer);
    
    /// <summary>
    /// Updates an existing customer
    /// </summary>
    Task UpdateAsync(CustomerDomainModel customer);
    
    /// <summary>
    /// Deletes a customer
    /// </summary>
    Task DeleteAsync(CustomerDomainModel customer);
}
