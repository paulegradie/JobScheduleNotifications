using System.Text.RegularExpressions;

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

    /// <summary>
    /// Render to standard cron expression string.
    /// </summary>
    public override string ToString()
        => $"{Minute} {Hour} {DayOfMonth} {Month} {DayOfWeek}";


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

/// <summary>
/// Fluent factory for building CronSchedule instances.
/// </summary>
public class FluentCron
{
    private string _minute = "*";
    private string _hour = "*";
    private string _dayOfMonth = "*";
    private string _month = "*";
    private string _dayOfWeek = "*";

    private FluentCron()
    {
    }

    /// <summary>Create a new FluentCron factory.</summary>
    public static FluentCron Create() => new FluentCron();

    public FluentCron EveryMinute()
    {
        _minute = "*";
        return this;
    }

    public FluentCron AtMinute(int minute)
    {
        if (minute < 0 || minute > 59)
            throw new ArgumentOutOfRangeException(nameof(minute));
        _minute = minute.ToString();
        return this;
    }

    public FluentCron EveryHour()
    {
        _hour = "*";
        return this;
    }

    public FluentCron AtHour(int hour)
    {
        if (hour < 0 || hour > 23)
            throw new ArgumentOutOfRangeException(nameof(hour));
        _hour = hour.ToString();
        return this;
    }

    public FluentCron OnDayOfMonth(int day)
    {
        if (day < 1 || day > 31)
            throw new ArgumentOutOfRangeException(nameof(day));
        _dayOfMonth = day.ToString();
        return this;
    }

    public FluentCron EveryDays(int days)
    {
        if (days is < 1 or > 31)
            throw new ArgumentOutOfRangeException(nameof(days));

        _dayOfMonth = $"*/{days}";
        return this;
    }

    public FluentCron EveryWeeks(int weeks)
    {
        if (weeks < 1) throw new ArgumentOutOfRangeException(nameof(weeks));
        _dayOfMonth = $"*/{weeks * 7}";
        return this;

    }

    public FluentCron InMonth(int month)
    {
        if (month < 1 || month > 12)
            throw new ArgumentOutOfRangeException(nameof(month));
        _month = month.ToString();
        return this;
    }

    public FluentCron EveryMonths(int months)
    {
        if (months < 1 || months > 12)
            throw new ArgumentOutOfRangeException(nameof(months));
        _month = $"*/{months}";
        return this;
    }

    public FluentCron OnDayOfWeek(int dayOfWeek)
    {
        if (dayOfWeek < 0 || dayOfWeek > 6)
            throw new ArgumentOutOfRangeException(nameof(dayOfWeek));
        _dayOfWeek = dayOfWeek.ToString();
        return this;
    }

    /// <summary>
    /// Build the CronSchedule domain object.
    /// </summary>
    public CronSchedule Build()
        => new CronSchedule(_minute, _hour, _dayOfMonth, _month, _dayOfWeek);
}