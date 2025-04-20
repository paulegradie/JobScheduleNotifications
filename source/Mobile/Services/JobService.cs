using JobScheduleNotifications.Contracts;

namespace Mobile.Services;

public class JobService : IJobService
{
    private readonly HttpClientService _httpClient;
    private const string baseEndpoint = "jobs";

    public JobService(HttpClientService httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<JobDto>> GetJobsAsync()
    {
        return await _httpClient.GetListAsync<JobDto>(baseEndpoint) ?? [];
    }

    public async Task<JobDto> GetJobByIdAsync(Guid id)
    {
        return await _httpClient.GetAsync<JobDto>($"{baseEndpoint}/{id}") 
            ?? throw new InvalidOperationException($"Job with ID {id} not found");
    }

    public async Task<JobDto> CreateJobAsync(CreateJobDto job)
    {
        return await _httpClient.PostAsync<JobDto>(baseEndpoint, job) 
            ?? throw new InvalidOperationException("Failed to create job");
    }

    public async Task<JobDto> UpdateJobAsync(Guid id, UpdateJobDto job)
    {
        return await _httpClient.PutAsync<JobDto>($"{baseEndpoint}/{id}", job) 
            ?? throw new InvalidOperationException($"Failed to update job with ID {id}");
    }

    public async Task DeleteJobAsync(Guid id)
    {
        await _httpClient.DeleteAsync($"{baseEndpoint}/{id}");
    }

    public async Task<JobDto> MarkJobAsCompletedAsync(Guid id)
    {
        return await _httpClient.PutAsync<JobDto>($"{baseEndpoint}/{id}/complete", new { }) 
            ?? throw new InvalidOperationException($"Failed to mark job with ID {id} as completed");
    }

    public async Task<IEnumerable<JobDto>> GetJobsByCustomerAsync(Guid customerId)
    {
        return await _httpClient.GetListAsync<JobDto>($"{baseEndpoint}/customer/{customerId}") 
            ?? [];
    }

    public async Task<IEnumerable<JobDto>> GetJobsByBusinessOwnerAsync(Guid businessOwnerId)
    {
        return await _httpClient.GetListAsync<JobDto>($"{baseEndpoint}/business-owner/{businessOwnerId}") 
            ?? [];
    }
} 