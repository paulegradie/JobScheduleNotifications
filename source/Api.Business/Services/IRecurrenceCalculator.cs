using Server.Contracts.Cron;

namespace Api.Business.Services;

public interface IRecurrenceCalculator
{
    /// <summary>
    /// Given a recurrence pattern, its anchor date, and the last occurrence,
    /// return the next DateTime (in UTC).
    /// </summary>
    public DateTime GetNextOccurrence(CronSchedule schedule, DateTime anchorDate);
}