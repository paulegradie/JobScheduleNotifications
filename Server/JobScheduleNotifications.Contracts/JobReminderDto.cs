using System.ComponentModel.DataAnnotations;

namespace JobScheduleNotifications.Contracts;

public class JobReminderDto
{
    [Required(ErrorMessage = "ID is required")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Scheduled job ID is required")]
    public Guid ScheduledJobId { get; set; }

    [Required(ErrorMessage = "Reminder time is required")]
    [DataType(DataType.DateTime)]
    public DateTime ReminderTime { get; set; }

    [Required(ErrorMessage = "Message is required")]
    [MinLength(1, ErrorMessage = "Message cannot be empty")]
    [MaxLength(500, ErrorMessage = "Message cannot exceed 500 characters")]
    public string Message { get; set; } = string.Empty;

    [Required(ErrorMessage = "IsSent status is required")]
    public bool IsSent { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? SentDate { get; set; }
} 