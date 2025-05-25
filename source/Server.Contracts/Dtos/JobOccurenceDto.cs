using Api.ValueTypes;

namespace Server.Contracts.Dtos;

/// <summary>
/// Data-transfer object for a single job occurrence, including its reminders.
/// </summary>
public record JobOccurrenceDto(
    JobOccurrenceId JobOccurrenceId,
    ScheduledJobDefinitionId ScheduledJobDefinitionId,
    CustomerId CustomerId,
    DateTime OccurrenceDate,
    DateTime? CompletedDate,
    string JobTitle,
    string JobDescription,
    bool MarkedAsCompleted);