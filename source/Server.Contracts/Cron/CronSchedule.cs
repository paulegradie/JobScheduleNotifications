using System.Text.RegularExpressions;
using NCrontab;

namespace Server.Contracts.Cron;

/// <summary>
/// Domain object representing a cron schedule with strongly typed fields.
/// </summary>
public partial class CronSchedule
{
    public string Minute { get; }
    public string Hour { get; }
    public string DayOfMonth { get; }
    public string Month { get; }
    public string DayOfWeek { get; }

    public CronSchedule(string minute, string hour, string dayOfMonth, string month, string dayOfWeek)
    {
        Minute = minute;
        Hour = hour;
        DayOfMonth = dayOfMonth;
        Month = month;
        DayOfWeek = dayOfWeek;
    }

    public CronSchedule(CrontabSchedule cronSchedule)
    {
        if (cronSchedule == null) throw new ArgumentNullException(nameof(cronSchedule));

        // Use the standard cron string representation and split into fields
        var expr = cronSchedule.ToString();
        var parts = expr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 5)
            throw new ArgumentException(
                $"Expected 5-part cron expression but got {parts.Length} parts: '{expr}'",
                nameof(cronSchedule));

        Minute = parts[0];
        Hour = parts[1];
        DayOfMonth = parts[2];
        Month = parts[3];
        DayOfWeek = parts[4];
    }

    /// <summary>
    /// Render to standard cron expression string.
    /// </summary>
    public override string ToString()
        => $"{Minute} {Hour} {DayOfMonth} {Month} {DayOfWeek}";

    public CrontabSchedule ToCrontabSchedule() => CrontabSchedule.Parse(ToString());

    /// <summary>
    /// Given a start time and a current time, returns true if the interval
    /// specified by this cron schedule has fully elapsed.
    /// Supports simple intervals defined via "*/N" in day-of-month or month fields.
    /// </summary>
    public bool HasIntervalElapsed(DateTime start, DateTime now)
    {
        if (now < start)
            return false;

        var regex = MyRegex();
        // Check for day-of-month step (e.g., every N days/weeks)
        var domMatch = regex.Match(DayOfMonth);
        if (domMatch.Success)
        {
            int days = int.Parse(domMatch.Groups[1].Value);
            return (now - start) >= TimeSpan.FromDays(days);
        }

        // Check for month step (e.g., every N months)
        var mMatch = regex.Match(Month);
        if (mMatch.Success)
        {
            var months = int.Parse(mMatch.Groups[1].Value);
            var target = start.AddMonths(months);
            return now >= target;
        }

        throw new NotSupportedException("Only interval schedules using '*/N' in day-of-month or month fields are supported.");
    }

    [GeneratedRegex(@"^\*/(\d+)$")]
    private static partial Regex MyRegex();
}