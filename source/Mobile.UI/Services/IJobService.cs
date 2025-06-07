using System.Collections.Generic;
using System.Threading.Tasks;
using Api.ValueTypes;
using Server.Contracts.Dtos;

namespace Mobile.UI.Services;

public interface IJobService
{
    Task CreateJobAsync(CreateScheduledJobDefinitionDto job);
    Task<IEnumerable<ScheduledJobDefinitionDto>> GetJobsAsync(CustomerId customerId);
    Task<ScheduledJobDefinitionDto> GetJobAsync(CustomerId customerId, ScheduledJobDefinitionId scheduledJobDefinitionId);
    Task<ScheduledJobDefinitionDto> UpdateJobAsync(ScheduledJobDefinitionDto scheduledJobDefinitionId);
}