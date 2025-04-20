using System.Net.Http.Json;
using JobScheduleNotifications.Contracts;

namespace Mobile.Services;

#pragma warning disable CA1063
public class ApiService : IApiService, IDisposable
#pragma warning restore CA1063
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;
    private const string baseUrl = "https://localhost:5001/api"; // We'll make this configurable later

    public ApiService(IAuthService authService)
    {
        _httpClient = new HttpClient();
        _authService = authService;
    }

    private async Task<string?> GetAuthTokenAsync()
    {
        return await _authService.GetAuthTokenAsync();
    }

    public async Task<T?> GetAsync<T>(string endpoint, string? authToken = null)
    {
        try
        {
            var token = authToken ?? await GetAuthTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.GetAsync(new Uri($"{baseUrl}/{endpoint}"));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (Exception ex)
        {
            // Log the error
            System.Diagnostics.Debug.WriteLine($"API Error (GET): {ex.Message}");
            throw;
        }
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data, string? authToken = null)
    {
        try
        {
            var token = authToken ?? await GetAuthTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/{endpoint}", data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"API Error (POST): {ex.Message}");
            throw;
        }
    }

    public async Task<T?> PutAsync<T>(string endpoint, object data, string? authToken = null)
    {
        try
        {
            var token = authToken ?? await GetAuthTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.PutAsJsonAsync($"{baseUrl}/{endpoint}", data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"API Error (PUT): {ex.Message}");
            throw;
        }
    }

    public async Task<bool> DeleteAsync(string endpoint, string? authToken = null)
    {
        try
        {
            var token = authToken ?? await GetAuthTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.DeleteAsync(new Uri($"{baseUrl}/{endpoint}"));
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"API Error (DELETE): {ex.Message}");
            throw;
        }
    }

    // Scheduled Jobs
    public async Task<List<ScheduledJobDto>> GetScheduledJobsAsync()
    {
        var result = await GetAsync<List<ScheduledJobDto>>("scheduledjobs");
        return result ?? new List<ScheduledJobDto>();
    }

    public async Task<ScheduledJobDto> CreateScheduledJobAsync(CreateScheduledJobDto job)
    {
        var result = await PostAsync<ScheduledJobDto>("scheduledjobs", job);
        return result ?? throw new Exception("Failed to create scheduled job");
    }

    public async Task<bool> UpdateScheduledJobAsync(ScheduledJobDto job)
    {
        await PutAsync<ScheduledJobDto>($"scheduledjobs/{job.Id}", job);
        return true;
    }

    public async Task<bool> DeleteScheduledJobAsync(Guid jobId)
    {
        return await DeleteAsync($"scheduledjobs/{jobId}");
    }

    // Job Reminders
    public async Task<List<JobReminderDto>> GetJobRemindersAsync(Guid jobId)
    {
        var result = await GetAsync<List<JobReminderDto>>($"scheduledjobs/{jobId}/reminders");
        return result ?? new List<JobReminderDto>();
    }

    public async Task<JobReminderDto> CreateJobReminderAsync(CreateJobReminderDto reminder)
    {
        var result = await PostAsync<JobReminderDto>($"scheduledjobs/{reminder.ScheduledJobId}/reminders", reminder);
        return result ?? throw new Exception("Failed to create job reminder");
    }

    public async Task<bool> UpdateJobReminderAsync(JobReminderDto reminder)
    {
        await PutAsync<JobReminderDto>($"scheduledjobs/{reminder.ScheduledJobId}/reminders/{reminder.Id}", reminder);
        return true;
    }

    public async Task<bool> DeleteJobReminderAsync(Guid reminderId)
    {
        return await DeleteAsync($"jobreminders/{reminderId}");
    }

#pragma warning disable CA1063
    public void Dispose()
#pragma warning restore CA1063
    {
          _httpClient.Dispose();
          GC.SuppressFinalize(this);
      }
} 