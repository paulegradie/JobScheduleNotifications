using Api.Infrastructure.DbTables.OrganizationModels;
using Api.Infrastructure.EntityFramework;
using Api.ValueTypes;

namespace Api.Infrastructure.DbTables;

[DatabaseModel]
public class ScheduledJob
{
    public ScheduledJobId Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public CustomerId CustomerId { get; set; }
    public virtual Customer Customer { get; set; } = null!;
    public virtual ICollection<JobReminder> Reminders { get; } = new List<JobReminder>();
}