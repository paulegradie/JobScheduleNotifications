namespace Server.Contracts.Dtos;

public class CreateScheduledJobDto
{
    public Guid CustomerId { get; set; }

    public Guid BusinessOwnerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public DateTime ScheduledStartTime { get; set; }

    public double EstimatedDurationHours { get; set; }

    public decimal Price { get; set; }

    public string? Notes { get; set; }
}