using Server.Contracts.Dtos;

namespace Mobile.UI.Services;

public interface IJobService
{
    Task CreateJobAsync(CreateScheduledJobDefinitionDto job);
}