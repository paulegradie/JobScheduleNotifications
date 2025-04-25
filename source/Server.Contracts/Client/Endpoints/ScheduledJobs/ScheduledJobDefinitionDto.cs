using Api.ValueTypes.Enums;

namespace Server.Contracts.Client.Endpoints.ScheduledJobs;

public record ScheduledJobDefinitionDto(
    Guid Id,
    string Title,
    string Description,
    Frequency Frequency,
    int Interval,
    DayOfWeek[]? DaysOfWeek,
    int? DayOfMonth,
    string? CronExpression
);