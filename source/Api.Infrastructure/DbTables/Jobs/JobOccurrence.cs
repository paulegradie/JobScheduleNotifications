using System.ComponentModel.DataAnnotations.Schema;
using Api.Infrastructure.DbTables.OrganizationModels;
using Api.ValueTypes;

namespace Api.Infrastructure.DbTables.Jobs;

public class JobOccurrence
{
    public JobOccurrenceId JobOccurrenceId { get; set; }
    public DateTime OccurrenceDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public DateTime? InvoicePaidDate { get; set; }

    [NotMapped] public bool MarkedAsCompleted => CompletedDate.HasValue;
    [NotMapped] public bool InvoiceWasPaid => InvoicePaidDate.HasValue;

    
    //UP
    public CustomerId CustomerId { get; set; }
    public virtual Customer Customer { get; set; } = null!;
    public virtual ScheduledJobDefinition ScheduledJobDefinition { get; set; } = null!;
    public ScheduledJobDefinitionId ScheduledJobDefinitionId { get; set; }

    // DOWN
    public JobOccurrenceStatus JobOccurrenceStatus { get; set; }
    public JobOccurenceInvoiceStatus JobOccurenceInvoiceStatus { get; set; }
    public virtual ICollection<JobCompletedPhoto> JobCompletedPhotos { get; set; }

}

public enum JobOccurrenceStatus
{
    Canceled = 0,
    NotStarted = 1,
    InProgress = 2,
    Completed = 3,
}

public enum JobOccurenceInvoiceStatus
{
    Issued,
    Paid
}