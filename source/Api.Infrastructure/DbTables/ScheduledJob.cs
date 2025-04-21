using Api.Infrastructure.EntityFramework;

namespace Api.Infrastructure.DbTables;

[DatabaseModel]
public class ScheduledJob
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public Guid CustomerId { get; set; }
    public virtual Customer Customer { get; set; } = null!;
    public Guid BusinessOwnerId { get; set; }
    public virtual BusinessOwner BusinessOwner { get; set; } = null!;
    public virtual ICollection<JobReminder> Reminders { get; } = new List<JobReminder>();
} 