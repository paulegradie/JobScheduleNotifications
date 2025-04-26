using System.ComponentModel.DataAnnotations;

namespace Server.Contracts.Dtos;

public class CreateJobReminderDto
{
    public Guid ScheduledJobId { get; set; }

    public DateTime ReminderTime { get; set; }

    public string Message { get; set; } = string.Empty;
}