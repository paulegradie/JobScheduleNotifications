using System.ComponentModel.DataAnnotations;

namespace JobScheduleNotifications.Contracts;

public class UpdateJobDto
{
    [MinLength(2, ErrorMessage = "Job title must be at least 2 characters long")]
    [MaxLength(100, ErrorMessage = "Job title cannot exceed 100 characters")]
    public string? Title { get; set; }

    [MinLength(10, ErrorMessage = "Job description must be at least 10 characters long")]
    [MaxLength(1000, ErrorMessage = "Job description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? ScheduledDate { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? CompletedDate { get; set; }

    public string? Status { get; set; }

    [Range(0, 10000, ErrorMessage = "Price must be between 0 and 10000")]
    public decimal? Price { get; set; }

    public string? Notes { get; set; }
} 