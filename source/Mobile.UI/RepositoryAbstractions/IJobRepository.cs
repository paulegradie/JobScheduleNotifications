using Server.Contracts.Dtos;

namespace Mobile.UI.RepositoryAbstractions;

public interface IJobRepository
{
    Task<IEnumerable<JobDto>> GetJobsAsync();
    Task<JobDto> GetJobByIdAsync(Guid id);
    Task<JobDto> CreateJobAsync(CreateJobDto job);
    Task<JobDto> UpdateJobAsync(Guid id, UpdateJobDto job);
    Task DeleteJobAsync(Guid id);
    Task<JobDto> MarkJobAsCompletedAsync(Guid id);
    Task<IEnumerable<JobDto>> GetJobsByCustomerAsync(Guid customerId);
    Task<IEnumerable<JobDto>> GetJobsByBusinessOwnerAsync(Guid businessOwnerId);
} 