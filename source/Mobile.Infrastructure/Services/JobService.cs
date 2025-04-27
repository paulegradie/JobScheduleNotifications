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
}