using Api.Business.Entities;
using Api.ValueTypes;

namespace Api.Business.Repositories;

public interface IInvoiceRepository
{
    /// <summary>
    /// Saves an invoice to the database
    /// </summary>
    Task<InvoiceDomainModel> SaveInvoiceAsync(InvoiceDomainModel invoice);
    
    /// <summary>
    /// Gets an invoice by its ID
    /// </summary>
    Task<InvoiceDomainModel?> GetByIdAsync(InvoiceId invoiceId);
    
    /// <summary>
    /// Gets all invoices for a specific job occurrence
    /// </summary>
    Task<IEnumerable<InvoiceDomainModel>> GetByJobOccurrenceAsync(JobOccurrenceId jobOccurrenceId);
    
    /// <summary>
    /// Gets all invoices for a specific customer
    /// </summary>
    Task<IEnumerable<InvoiceDomainModel>> GetByCustomerAsync(CustomerId customerId);
    
    /// <summary>
    /// Updates an existing invoice
    /// </summary>
    Task UpdateAsync(InvoiceDomainModel invoice);
    
    /// <summary>
    /// Deletes an invoice
    /// </summary>
    Task DeleteAsync(InvoiceDomainModel invoice);
}
