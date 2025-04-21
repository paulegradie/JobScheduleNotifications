namespace Server.Contracts.Dtos
{
    public class UpdateScheduledJobDto
    {
        public string? JobDetails { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public bool? IsCompleted { get; set; }
    }
} 