using Api.Business.Entities;
using Api.ValueTypes;

namespace Api.Business.Repositories;

/// <summary>
/// A repository for querying and managing job‐reminder domain models.
/// </summary>
public interface IJobReminderRepository
{
    /// <summary>
    /// Returns all reminders for a given scheduled‐job definition.
    /// Optionally filter by whether they have been sent.
    /// </summary>
    Task<IEnumerable<JobReminderDomainModel>> ListByJobDefinitionAsync(
        ScheduledJobDefinitionId scheduledJobDefinitionId,
        bool? isSent = null);

    /// <summary>
    /// Returns all reminders for all jobs belonging to a given customer.
    /// Optionally filter by whether they have been sent.
    /// </summary>
    Task<IEnumerable<JobReminderDomainModel>> ListByCustomerAsync(
        CustomerId customerId,
        bool? isSent = null);

    /// <summary>
    /// Returns all reminders that have not yet been sent.
    /// </summary>
    Task<IEnumerable<JobReminderDomainModel>> ListUnsentAsync(ScheduledJobDefinitionId scheduledJobDefinitionId);

    /// <summary>
    /// Returns all reminders whose ReminderDateTime is on or before the specified cutoff.
    /// </summary>
    Task<IEnumerable<JobReminderDomainModel>> ListDueAsync(DateTime upTo);

    /// <summary>
    /// Retrieves a single reminder by its Id.
    /// </summary>
    Task<JobReminderDomainModel?> GetByIdAsync(JobReminderId id);

    /// <summary>
    /// Adds a new reminder to the underlying store.
    /// </summary>
    Task AddAsync(JobReminderDomainModel reminder);

    /// <summary>
    /// Updates an existing reminder.
    /// </summary>
    Task UpdateAsync(JobReminderDomainModel reminder);

    /// <summary>
    /// Deletes a reminder.
    /// </summary>
    Task DeleteAsync(JobReminderDomainModel reminder);
}