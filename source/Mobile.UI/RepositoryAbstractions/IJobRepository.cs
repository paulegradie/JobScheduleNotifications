using Api.ValueTypes;
using Server.Contracts.Dtos;
using Server.Contracts.Endpoints;

namespace Mobile.UI.RepositoryAbstractions;

public interface IJobRepository
{
    Task<OperationResult<ScheduledJobDefinitionDto>> GetJobByIdAsync(
        CustomerId customerId,
        ScheduledJobDefinitionId id,
        CancellationToken ct = default);

    Task<OperationResult<ScheduledJobDefinitionDto>> CreateJobAsync(
        CreateScheduledJobDefinitionDto job,
        CancellationToken ct = default);

    Task<OperationResult<ScheduledJobDefinitionDto>> UpdateJobAsync(
        CustomerId customerId,
        ScheduledJobDefinitionId id,
        UpdateJobDto job,
        CancellationToken ct = default);

    Task<OperationResult> DeleteJobAsync(
        ScheduledJobDefinitionId id,
        CancellationToken ct = default);

    
    Task<OperationResult<IEnumerable<ScheduledJobDefinitionDto>>> GetJobsByCustomerAsync(
        CustomerId customerId,
        CancellationToken ct = default);
}