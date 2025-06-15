using Api.Business.Entities;
using Api.Business.Repositories;
using Api.Infrastructure.Data;
using Api.Infrastructure.Repositories.Mapping;
using Api.ValueTypes;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly AppDbContext _context;

    public InvoiceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<InvoiceDomainModel> SaveInvoiceAsync(InvoiceDomainModel invoice)
    {
        var entity = invoice.ToDbEntity();
        _context.Invoices.Add(entity);
        await _context.SaveChangesAsync();
        
        return entity.ToDomainModel();
    }

    public async Task<InvoiceDomainModel?> GetByIdAsync(InvoiceId invoiceId)
    {
        var entity = await _context.Invoices
            .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
            
        return entity?.ToDomainModel();
    }

    public async Task<IEnumerable<InvoiceDomainModel>> GetByJobOccurrenceAsync(JobOccurrenceId jobOccurrenceId)
    {
        var entities = await _context.Invoices
            .Where(i => i.JobOccurrenceId == jobOccurrenceId)
            .ToListAsync();
            
        return entities.Select(e => e.ToDomainModel());
    }

    public async Task<IEnumerable<InvoiceDomainModel>> GetByCustomerAsync(CustomerId customerId)
    {
        var entities = await _context.Invoices
            .Where(i => i.CustomerId == customerId)
            .ToListAsync();
            
        return entities.Select(e => e.ToDomainModel());
    }

    public async Task UpdateAsync(InvoiceDomainModel invoice)
    {
        var entity = await _context.Invoices
            .FirstOrDefaultAsync(i => i.InvoiceId == invoice.InvoiceId);
            
        if (entity != null)
        {
            entity.UpdateFromDomainModel(invoice);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(InvoiceDomainModel invoice)
    {
        var entity = await _context.Invoices
            .FirstOrDefaultAsync(i => i.InvoiceId == invoice.InvoiceId);
            
        if (entity != null)
        {
            _context.Invoices.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
