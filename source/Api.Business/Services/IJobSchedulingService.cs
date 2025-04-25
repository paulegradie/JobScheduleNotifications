namespace Api.Business.Services;

public interface IJobSchedulingService
{
    /// <summary>
    /// Scan all definitions, create any due occurrences + reminders, and persist changes.
    /// </summary>
    Task ProcessDueJobsAsync();
}