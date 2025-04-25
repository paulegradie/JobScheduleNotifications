using Api.ValueTypes.Enums;

namespace Server.Contracts.Client.Endpoints.Customers.Contracts;

public record UpdateJobDefinitionRequest(
    string Title,
    string Description,
    Frequency Frequency,
    int Interval,
    DayOfWeek[]? DaysOfWeek,
    int? DayOfMonth,
    string? CronExpression
);