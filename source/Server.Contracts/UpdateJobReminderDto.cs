namespace JobScheduleNotifications.Contracts
{
    public class UpdateJobReminderDto
    {
        public DateTime? ReminderTime { get; set; }
        public string? Message { get; set; }
        public bool? IsSent { get; set; }
    }
} 