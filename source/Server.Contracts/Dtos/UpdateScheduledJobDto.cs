namespace Server.Contracts
{
    public class UpdateScheduledJobDto
    {
        public string? JobDetails { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public bool? IsCompleted { get; set; }
    }
} 