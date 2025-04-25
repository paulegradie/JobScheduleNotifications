using Api.Infrastructure.DbTables.OrganizationModels;
using Api.ValueTypes;

namespace Api.Infrastructure.DbTables.Jobs;

public class JobOccurrence
{
    public JobOccurrenceId Id { get; set; }
    public ScheduledJobDefinitionId DefinitionId { get; set; }
    public DateTime OccurrenceDate { get; set; }
    public DateTime? CompletedDate { get; set; }

    public CustomerId CustomerId { get; set; }
    public virtual Customer Customer { get; set; } = null!;

    public virtual ScheduledJobDefinition Definition { get; set; } = null!;

    // if you want per‐occurrence reminders
    public virtual ICollection<JobReminder> JobReminders { get; set; } = new List<JobReminder>();
}