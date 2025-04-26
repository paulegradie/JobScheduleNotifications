using Server.Contracts.Client.Endpoints.JobOccurence.Contracts;

namespace Server.Contracts.Client.Endpoints.JobOccurence;

/// <summary>
/// Client-side interface for calling JobOccurrences endpoints.
/// </summary>
public interface IJobOccurrencesEndpoint
{
    /// <summary>
    /// Lists all occurrences for a given job definition.
    /// </summary>
    Task<OperationResult<ListJobOccurrencesResponse>> ListAsync(
        ListJobOccurrencesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single occurrence by ID.
    /// </summary>
    Task<OperationResult<GetJobOccurrenceByIdResponse>> GetAsync(
        GetJobOccurrenceByIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new job occurrence.
    /// </summary>
    Task<OperationResult<CreateJobOccurrenceResponse>> CreateAsync(
        CreateJobOccurrenceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing job occurrence.
    /// </summary>
    Task<OperationResult<UpdateJobOccurrenceResponse>> UpdateAsync(
        UpdateJobOccurrenceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an existing job occurrence.
    /// </summary>
    Task<OperationResult<DeleteJobOccurrenceResponse>> DeleteAsync(
        DeleteJobOccurrenceRequest request,
        CancellationToken cancellationToken = default);
}