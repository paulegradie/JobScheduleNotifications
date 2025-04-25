using Api.ValueTypes;

namespace Server.Contracts.Dtos;

public class JobReminderDto
{
    public JobReminderId Id { get; set; }

    public ScheduledJobDefinitionId ScheduledJobDefinitionId { get; set; }

    public DateTime ReminderTime { get; set; }

    public string Message { get; set; } = string.Empty;

    public bool IsSent { get; set; }

    public DateTime? SentDate { get; set; }
}