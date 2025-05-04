using Api.ValueTypes;
using Mobile.UI.RepositoryAbstractions;
using Mobile.UI.Services;
using Server.Contracts.Dtos;

namespace Mobile.Infrastructure.Services;

internal class JobService : IJobService
{
    private readonly IJobRepository _jobRepository;

    public JobService(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public async Task CreateJobAsync(CreateScheduledJobDefinitionDto job)
    {
        var result = await _jobRepository.CreateJobAsync(job);
        if (!result.IsSuccess)
            throw new Exception(result.ErrorMessage ?? "Failed to schedule job.");
    }

    public async Task<IEnumerable<ScheduledJobDefinitionDto>> GetJobsAsync(CustomerId customerId)
    {
        var result = await _jobRepository.GetJobsByCustomerAsync(customerId);
        if (result.IsSuccess)
            return result.Value;
        else
        {
            throw new Exception(result.ErrorMessage ?? "Failed to load jobs.");
        }
    }
    public async Task<ScheduledJobDefinitionDto> GetJobAsync(CustomerId customerId, ScheduledJobDefinitionId scheduledJobDefinitionId)
    {
        var result = await _jobRepository.GetJobByIdAsync(customerId, scheduledJobDefinitionId);
        if (result.IsSuccess)
        {
            return result.Value;
        }

        throw new Exception(result.ErrorMessage ?? "Failed to update job.");
    }

    public async Task<ScheduledJobDefinitionDto> UpdateJobAsync(ScheduledJobDefinitionDto scheduledJobDefinitionDto)
    {
        var updateDto = new UpdateJobDto
        {
            DayOfMonth = scheduledJobDefinitionDto.Pattern.DayOfMonth,
        };
        var result = await _jobRepository.UpdateJobAsync(scheduledJobDefinitionDto.CustomerId, scheduledJobDefinitionDto.ScheduledJobDefinitionId, updateDto);
        if (result.IsSuccess)
        {
            return result.Value;
        }

        throw new Exception(result.ErrorMessage ?? "Failed to update job.");
    }
}