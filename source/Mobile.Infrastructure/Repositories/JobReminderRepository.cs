using Api.ValueTypes;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts;
using Server.Contracts.Endpoints;
using Server.Contracts.Endpoints.JobOccurence.Contracts;
using Server.Contracts.Endpoints.Reminders.Contracts;

namespace Mobile.Infrastructure.Repositories;

internal class JobReminderRepository : IJobReminderRepository
{
    private readonly IServerClient _serverClient;

    public JobReminderRepository(IServerClient serverClient)
    {
        _serverClient = serverClient;
    }

    public async Task<OperationResult<GetJobReminderResponse>> GetReminderAsync(CustomerId customerId, ScheduledJobDefinitionId scheduledJobDefinitionId, JobOccurrenceId jobOccurrenceId,
        JobReminderId jobReminderId, CancellationToken cancellationToken)
    {
        var request = new GetJobReminderByIdRequest(customerId, scheduledJobDefinitionId, jobReminderId);
        var op = await _serverClient.JobReminders.GetJobReminderByIdAsync(request, cancellationToken);

        return !op.IsSuccess
            ? OperationResult<GetJobReminderResponse>.Failure(op.ErrorMessage ?? "Unknown error", op.StatusCode)
            : OperationResult<GetJobReminderResponse>.Success(new GetJobReminderResponse(op.Value.JobReminder), op.StatusCode);
    }
}