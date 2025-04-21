using Mobile.Core.Repositories;
using Server.Contracts;
using Server.Contracts.Client;
using Server.Contracts.Dtos;

namespace Mobile.Core.Services;

public class JobRepository : IJobRepository
{
    private readonly IServerClient _httpClient;
    private const string baseEndpoint = "jobs";

    public JobRepository(IServerClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<JobDto>> GetJobsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<JobDto> GetJobByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<JobDto> CreateJobAsync(CreateJobDto job)
    {
        throw new NotImplementedException();
    }

    public async Task<JobDto> UpdateJobAsync(Guid id, UpdateJobDto job)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteJobAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<JobDto> MarkJobAsCompletedAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<JobDto>> GetJobsByCustomerAsync(Guid customerId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<JobDto>> GetJobsByBusinessOwnerAsync(Guid businessOwnerId)
    {
        throw new NotImplementedException();
    }
}