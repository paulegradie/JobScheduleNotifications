using System.Threading;
using System.Threading.Tasks;
using Api.ValueTypes;
using Server.Contracts.Endpoints;
using Server.Contracts.Endpoints.JobOccurence.Contracts;

namespace Mobile.UI.RepositoryAbstractions;

/// <summary>
/// Repository abstraction for fetching Job Reminders.
/// </summary>
public interface IJobReminderRepository
{
    /// <summary>
    /// Fetches a single JobReminder by composite IDs.
    /// </summary>
    Task<OperationResult<GetJobReminderResponse>> GetReminderAsync(
        CustomerId customerId,
        ScheduledJobDefinitionId scheduledJobDefinitionId,
        JobOccurrenceId jobOccurrenceId,
        JobReminderId jobReminderId,
        CancellationToken cancellationToken);
}