using Api.ValueTypes;

namespace Server.Contracts.Dtos;

/// <summary>
/// Data-transfer object for a single job occurrence, including its reminders.
/// </summary>
public class JobOccurrenceDto
{
    /// <summary>
    /// Unique identifier for this occurrence.
    /// </summary>
    public JobOccurrenceId Id { get; set; }

    /// <summary>
    /// The scheduled job definition to which this occurrence belongs.
    /// </summary>
    public ScheduledJobDefinitionId ScheduledJobDefinitionId { get; set; }

    /// <summary>
    /// The date and time when this occurrence happens.
    /// </summary>
    public DateTime OccurrenceDate { get; set; }

    /// <summary>
    /// All reminders associated with this occurrence.
    /// </summary>
    public List<JobReminderDto> JobReminders { get; set; } = new();
}