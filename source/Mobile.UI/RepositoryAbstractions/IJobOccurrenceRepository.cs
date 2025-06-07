using System.Threading;
using System.Threading.Tasks;
using Api.ValueTypes;
using Server.Contracts.Endpoints;
using Server.Contracts.Endpoints.JobOccurence.Contracts;

namespace Mobile.UI.RepositoryAbstractions;

public interface IJobOccurrenceRepository
{
    Task<OperationResult<UpdateJobOccurrenceResponse>> MarkOccurrenceAsCompletedAsync(
        CustomerId customerId,
        ScheduledJobDefinitionId scheduledJobDefinitionId,
        JobOccurrenceId jobOccurrenceId,
        CancellationToken ct = default);

    Task<OperationResult<CreateJobOccurrenceResponse>> CreateNewOccurrence(
        CustomerId customerId,
        ScheduledJobDefinitionId scheduledJobDefinitionId,
        CancellationToken ct = default);

    Task<OperationResult<GetJobOccurrenceByIdResponse>> GetOccurrenceByIdAsync(
        CustomerId customerId,
        ScheduledJobDefinitionId scheduledJobDefinitionId,
        JobOccurrenceId id,
        CancellationToken ct = default);
}
