using Api.ValueTypes;

namespace Server.Contracts.Dtos;

public class JobReminderDto
{
    public JobReminderId JobReminderId { get; set; }

    public JobOccurrenceId JobOccurrenceId { get; set; }
    public ScheduledJobDefinitionId ScheduledJobDefinitionId { get; set; }

    public DateTime ReminderDateTime { get; set; }

    public string Message { get; set; } = string.Empty;

    public bool IsSent { get; set; }

    public DateTime? SentDate { get; set; }
}