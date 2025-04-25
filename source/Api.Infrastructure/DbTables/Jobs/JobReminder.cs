using Api.ValueTypes;

namespace Api.Infrastructure.DbTables.Jobs;

public class JobReminder
{
    public JobReminderId Id { get; set; }
    public DateTime ReminderDateTime { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsSent { get; set; }
    public DateTime? SentDate { get; set; }
    public virtual JobOccurrence JobOccurrence { get; set; } = null!;
    public JobOccurrenceId JobOccurrenceId { get; set; }
}