namespace JobScheduleNotifications.Core.Entities;

public class JobReminder
{
    public Guid Id { get; set; }
    public Guid ScheduledJobId { get; set; }
    public DateTime ReminderTime { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsSent { get; set; }
    public DateTime? SentDate { get; set; }
    public virtual ScheduledJob ScheduledJob { get; set; } = null!;
} 