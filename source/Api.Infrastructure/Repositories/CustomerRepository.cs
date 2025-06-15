using Api.Business.Entities;
using Api.Business.Repositories;
using Api.Infrastructure.Data;
using Api.Infrastructure.Repositories.Mapping;
using Api.ValueTypes;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerDomainModel?> GetByIdAsync(CustomerId customerId)
    {
        var entity = await _context.Customers
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);
            
        return entity?.ToDomainModel();
    }

    public async Task<IEnumerable<CustomerDomainModel>> GetByOrganizationAsync(OrganizationId organizationId)
    {
        var entities = await _context.Customers
            .Where(c => c.OrganizationId == organizationId)
            .ToListAsync();
            
        return entities.Select(e => e.ToDomainModel());
    }

    public async Task<CustomerDomainModel> CreateAsync(CustomerDomainModel customer)
    {
        var entity = customer.ToDbEntity();
        _context.Customers.Add(entity);
        await _context.SaveChangesAsync();
        
        return entity.ToDomainModel();
    }

    public async Task UpdateAsync(CustomerDomainModel customer)
    {
        var entity = await _context.Customers
            .FirstOrDefaultAsync(c => c.CustomerId == customer.CustomerId);
            
        if (entity != null)
        {
            entity.UpdateFromDomainModel(customer);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(CustomerDomainModel customer)
    {
        var entity = await _context.Customers
            .FirstOrDefaultAsync(c => c.CustomerId == customer.CustomerId);
            
        if (entity != null)
        {
            _context.Customers.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
