using System.ComponentModel.DataAnnotations;

namespace Server.Contracts.Dtos;

public class JobDto
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime ScheduledDate { get; set; }

    public DateTime? CompletedDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal Budget { get; set; }

    public string? Notes { get; set; }
}