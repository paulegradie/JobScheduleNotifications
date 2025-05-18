using Api.Infrastructure.DbTables.OrganizationModels;
using Api.ValueTypes;

namespace Api.Infrastructure.DbTables.Jobs;

public class JobOccurrence
{
    public JobOccurrenceId JobOccurrenceId { get; set; }
    public DateTime OccurrenceDate { get; set; }
    public DateTime? CompletedDate { get; set; }


    // relationships

    //UP
    public CustomerId CustomerId { get; set; }
    public virtual Customer Customer { get; set; } = null!;
    public virtual ScheduledJobDefinition ScheduledJobDefinition { get; set; } = null!;
    public ScheduledJobDefinitionId ScheduledJobDefinitionId { get; set; }

    // DOWN
    public virtual ICollection<JobReminder> JobReminders { get; set; } = new List<JobReminder>();
    
    public enum JobOccurrenceStatus
    {
        NotStarted,
        InProgress,
        Completed,
        Canceled
    }
}