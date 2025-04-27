using Server.Contracts.Endpoints.Reminders.Contracts;

namespace Server.Contracts.Endpoints.Reminders;

/// <summary>
/// Client-side interface for interacting with the Job Reminders API.
/// </summary>
public interface IJobRemindersEndpoint
{
    /// <summary>
    /// Retrieves a list of job reminders for a specified customer and job definition.
    /// </summary>
    Task<OperationResult<ListJobRemindersResponse>> ListJobRemindersAsync(
        ListJobRemindersRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a single job reminder by its ID.
    /// </summary>
    Task<OperationResult<GetJobReminderByIdResponse>> GetJobReminderByIdAsync(
        GetJobReminderByIdRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new job reminder.
    /// </summary>
    Task<OperationResult<CreateJobReminderResponse>> CreateJobReminderAsync(
        CreateJobReminderRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing job reminder.
    /// </summary>
    Task<OperationResult<UpdateJobReminderResponse>> UpdateJobReminderAsync(
        UpdateJobReminderRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an existing job reminder.
    /// </summary>
    Task<OperationResult> DeleteJobReminderAsync(
        DeleteJobReminderRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Acknowledges (marks as sent) a job reminder.
    /// </summary>
    Task<OperationResult<AcknowledgeJobReminderResponse>> AcknowledgeJobReminderAsync(
        AcknowledgeJobReminderRequest request,
        CancellationToken cancellationToken);
}