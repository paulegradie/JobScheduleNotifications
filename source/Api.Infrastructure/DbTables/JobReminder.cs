using Api.Infrastructure.EntityFramework;
using Api.ValueTypes;

namespace Api.Infrastructure.DbTables;

[DatabaseModel]
public class JobReminder
{
    public JobReminderId Id { get; set; }
    public ScheduledJobId ScheduledJobId { get; set; }
    public DateTime ReminderTime { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsSent { get; set; }
    public DateTime? SentDate { get; set; }
    public virtual ScheduledJob ScheduledJob { get; set; } = null!;
}