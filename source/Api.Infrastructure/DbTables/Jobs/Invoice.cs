using Api.Business.Services;
using Api.Infrastructure.DbTables.OrganizationModels;
using Api.ValueTypes;

namespace Api.Infrastructure.DbTables.Jobs;

public class Invoice
{
    public InvoiceId InvoiceId { get; set; }
    public JobOccurrenceId JobOccurrenceId { get; set; }
    public CustomerId CustomerId { get; set; }
    
    // File storage information
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public FileStorageLocation StorageLocation { get; set; }
    public long FileSizeBytes { get; set; }
    
    // Invoice metadata
    public DateTime CreatedDate { get; set; }
    public DateTime? EmailSentDate { get; set; }
    public string? EmailSentTo { get; set; }
    
    // Navigation properties
    public virtual JobOccurrence JobOccurrence { get; set; } = null!;
    public virtual Customer Customer { get; set; } = null!;
}


