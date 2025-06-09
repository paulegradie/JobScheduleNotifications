using Server.Contracts.Cron;

namespace IntegrationTests.ApiTests.Cron;

public class FluentCronTests
{
    [Fact]
    public void DefaultBuilder_ProducesWildcards()
    {
        var cron = FluentCron.Create().Build();
        Assert.Equal("* * * * *", cron.ToString());
    }

    [Theory]
    [InlineData(0, "0 * * * *")]
    [InlineData(15, "15 * * * *")]
    [InlineData(59, "59 * * * *")]
    public void AtMinute_SetsMinuteField(int minute, string expected)
    {
        var cron = FluentCron.Create()
            .AtMinute(minute)
            .Build();
        Assert.Equal(expected, cron.ToString());;
    }

    [Fact]
    public void AtMinute_InvalidMinute_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FluentCron.Create().AtMinute(60));
        Assert.Throws<ArgumentOutOfRangeException>(() => FluentCron.Create().AtMinute(-1));
    }

    [Theory]
    [InlineData(0, "* 0 * * *")]
    [InlineData(12, "* 12 * * *")]
    [InlineData(23, "* 23 * * *")]
    public void AtHour_SetsHourField(int hour, string expected)
    {
        var cron = FluentCron.Create()
            .AtHour(hour)
            .Build();
        Assert.Equal(expected, cron.ToString());
    }

    [Fact]
    public void AtHour_InvalidHour_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FluentCron.Create().AtHour(24));
        Assert.Throws<ArgumentOutOfRangeException>(() => FluentCron.Create().AtHour(-1));
    }

    [Theory]
    [InlineData(1, "* * 1 * *")]
    [InlineData(31, "* * 31 * *")]
    public void OnDayOfMonth_SetsDayField(int day, string expected)
    {
        var cron = FluentCron.Create()
            .OnDayOfMonth(day)
            .Build();
        Assert.Equal(expected, cron.ToString());
    }

    [Fact]
    public void OnDayOfMonth_Invalid_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FluentCron.Create().OnDayOfMonth(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => FluentCron.Create().OnDayOfMonth(32));
    }

    [Theory]
    [InlineData(1, "* * */1 * *")]
    [InlineData(10, "* * */10 * *")]
    [InlineData(31, "* * */31 * *")]
    public void EveryDays_SetsStepInDayOfMonth(int days, string expected)
    {
        var cron = FluentCron.Create()
            .EveryDays(days)
            .Build();
        Assert.Equal(expected, cron.ToString());
    }

    [Fact]
    public void EveryDays_Invalid_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FluentCron.Create().EveryDays(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => FluentCron.Create().EveryDays(32));
    }

    [Theory]
    [InlineData(1, "* * */7 * *")]
    [InlineData(6, "* * */42 * *")]
    public void EveryWeeks_EmulatesUsingEveryDays(int weeks, string expected)
    {
        var cron = FluentCron.Create()
            .EveryWeeks(weeks)
            .Build();
        Assert.Equal(expected, cron.ToString());
    }

    [Fact]
    public void EveryWeeks_Invalid_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FluentCron.Create().EveryWeeks(0));
    }

    [Theory]
    [InlineData(1, "* * * 1 *")]
    [InlineData(12, "* * * 12 *")]
    public void InMonth_SetsMonthField(int month, string expected)
    {
        var cron = FluentCron.Create()
            .InMonth(month)
            .Build();
        Assert.Equal(expected, cron.ToString());
    }

    [Fact]
    public void InMonth_Invalid_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FluentCron.Create().InMonth(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => FluentCron.Create().InMonth(13));
    }

    [Theory]
    [InlineData(1, "* * * */1 *")]
    [InlineData(4, "* * * */4 *")]
    [InlineData(12, "* * * */12 *")]
    public void EveryMonths_SetsStepInMonth(int months, string expected)
    {
        var cron = FluentCron.Create()
            .EveryMonths(months)
            .Build();
        Assert.Equal(expected, cron.ToString());
    }

    [Fact]
    public void EveryMonths_Invalid_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FluentCron.Create().EveryMonths(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => FluentCron.Create().EveryMonths(13));
    }

    [Theory]
    [InlineData(0, "* * * * 0")]
    [InlineData(6, "* * * * 6")]
    public void OnDayOfWeek_SetsDayOfWeekField(int dow, string expected)
    {
        var cron = FluentCron.Create()
            .OnDayOfWeek(dow)
            .Build();
        Assert.Equal(expected, cron.ToString());
    }

    [Fact]
    public void OnDayOfWeek_Invalid_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FluentCron.Create().OnDayOfWeek(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => FluentCron.Create().OnDayOfWeek(7));
    }
    
    // Scenario Tests

    [Fact]
    public void EverySixWeeks_ReturnsCorrectCron()
    {
        // Every 6 weeks = every 42 days at 00:00
        var cron = FluentCron.Create()
            .AtMinute(0)
            .AtHour(0)
            .EveryWeeks(6)
            .Build();
        Assert.Equal("0 0 */42 * *", cron.ToString());
    }

    [Fact]
    public void EveryFourMonths_ReturnsCorrectCron()
    {
        // Every 4 months on the 1st at midnight
        var cron = FluentCron.Create()
            .AtMinute(0)
            .AtHour(0)
            .OnDayOfMonth(1)
            .EveryMonths(4)
            .Build();
        Assert.Equal("0 0 1 */4 *", cron.ToString());
    }

    [Fact]
    public void Combined_TimeAndInterval_WorksCorrectly()
    {
        // At 3:30 every 10 days and January through December in step 2
        var cron = FluentCron.Create()
            .AtMinute(30)
            .AtHour(3)
            .EveryDays(10)
            .EveryMonths(2)
            .Build();
        Assert.Equal("30 3 */10 */2 *", cron.ToString());
    }
    [Fact]
    public void FluentCron_WithIntervalElapsed_WorksForWeeks()
    {
        var start = new DateTime(2025, 1, 1);
        var schedule = FluentCron.Create()
            .AtMinute(0)
            .AtHour(0)
            .EveryWeeks(6)
            .Build();

        // Before 42 days
        Assert.False(schedule.HasIntervalElapsed(start, start.AddDays(41)));
        // After 42 days
        Assert.True(schedule.HasIntervalElapsed(start, start.AddDays(42)));
    }

    [Fact]
    public void FluentCron_WithIntervalElapsed_WorksForMonths()
    {
        var start = new DateTime(2025, 1, 1);
        var schedule = FluentCron.Create()
            .AtMinute(0)
            .AtHour(0)
            .OnDayOfMonth(1)
            .EveryMonths(3)
            .Build();

        // Before 3 months
        Assert.False(schedule.HasIntervalElapsed(start, start.AddMonths(2).AddDays(30)));
        // After 3 months
        Assert.True(schedule.HasIntervalElapsed(start, start.AddMonths(3)));
    }
}

