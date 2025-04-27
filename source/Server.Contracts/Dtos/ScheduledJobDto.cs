using Api.ValueTypes;

namespace Server.Contracts.Dtos;

public class ScheduledJobDto
{
    public ScheduledJobDefinitionId Id { get; set; }

    public CustomerId CustomerId { get; set; }
    
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime ScheduledStartTime { get; set; }

    public double EstimatedDurationHours { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public decimal Price { get; set; }

    public string? Notes { get; set; }
}