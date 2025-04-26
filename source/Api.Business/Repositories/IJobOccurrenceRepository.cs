using Api.Business.Entities;
using Api.ValueTypes;

namespace Api.Business.Repositories;

/// <summary>
/// Repository abstraction for JobOccurrences.
/// </summary>
public interface IJobOccurrenceRepository
{
    /// <summary>
    /// Returns all occurrences for a given job definition.
    /// </summary>
    Task<IEnumerable<JobOccurrenceDomainModel>> ListByDefinitionAsync(ScheduledJobDefinitionId jobDefinitionId);

    /// <summary>
    /// Retrieves a single occurrence by its ID.
    /// </summary>
    Task<JobOccurrenceDomainModel?> GetByIdAsync(JobOccurrenceId occurrenceId);

    /// <summary>
    /// Adds a new job occurrence.
    /// </summary>
    Task AddAsync(JobOccurrenceDomainModel occurrence);

    /// <summary>
    /// Updates an existing job occurrence.
    /// </summary>
    Task UpdateAsync(JobOccurrenceDomainModel occurrence);

    /// <summary>
    /// Deletes a job occurrence.
    /// </summary>
    Task DeleteAsync(JobOccurrenceDomainModel occurrence);
}