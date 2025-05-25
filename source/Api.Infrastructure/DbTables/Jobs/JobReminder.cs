using Api.ValueTypes;

namespace Api.Infrastructure.DbTables.Jobs;

public class JobReminder
{
    public JobReminderId JobReminderId { get; set; }
    public DateTime ReminderDateTime { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsSent { get; set; }
    public DateTime? SentDate { get; set; }
    public virtual ScheduledJobDefinition ScheduledJobDefinition { get; set; } = null!;
    public ScheduledJobDefinitionId ScheduledJobDefinitionId { get; set; }
}