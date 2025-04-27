using Api.ValueTypes;

namespace Server.Contracts.Dtos;

public class CreateJobDto
{
    public CustomerId CustomerId { get; set; }
    
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime ScheduledDate { get; set; }

    public decimal Price { get; set; }

    public string? Notes { get; set; }
} 