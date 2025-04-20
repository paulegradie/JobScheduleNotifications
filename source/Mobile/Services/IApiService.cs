using JobScheduleNotifications.Contracts;

namespace Mobile.Services;

public interface IApiService
{
    Task<T?> GetAsync<T>(string endpoint, string? authToken = null);
    Task<T?> PostAsync<T>(string endpoint, object data, string? authToken = null);
    Task<T?> PutAsync<T>(string endpoint, object data, string? authToken = null);
    Task<bool> DeleteAsync(string endpoint, string? authToken = null);
    
    // Specific API methods
    Task<List<ScheduledJobDto>> GetScheduledJobsAsync();
    Task<ScheduledJobDto> CreateScheduledJobAsync(CreateScheduledJobDto job);
    Task<bool> UpdateScheduledJobAsync(ScheduledJobDto job);
    Task<bool> DeleteScheduledJobAsync(Guid jobId);
    
    Task<List<JobReminderDto>> GetJobRemindersAsync(Guid jobId);
    Task<JobReminderDto> CreateJobReminderAsync(CreateJobReminderDto reminder);
    Task<bool> UpdateJobReminderAsync(JobReminderDto reminder);
    Task<bool> DeleteJobReminderAsync(Guid reminderId);
} 