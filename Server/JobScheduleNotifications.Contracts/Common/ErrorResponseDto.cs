namespace JobScheduleNotifications.Contracts.Common;

public class ErrorResponseDto
{
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public int StatusCode { get; set; }
    public Dictionary<string, string[]> ValidationErrors { get; } = new();
} 