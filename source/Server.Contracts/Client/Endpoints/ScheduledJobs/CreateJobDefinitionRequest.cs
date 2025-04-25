using Api.ValueTypes.Enums;
using Server.Contracts.Common.Request;

namespace Server.Contracts.Client.Endpoints.ScheduledJobs;

public sealed record CreateScheduledJobDefinitionResponse(ScheduledJobDefinitionDto JobDefinition);


// TODO: May need to split this into two requests - one with anchor date and one without?
public record CreateJobDefinitionRequest : RequestBase
{
    public CreateJobDefinitionRequest(string Title,
        string Description,
        Frequency Frequency,
        int Interval,
        DayOfWeek[]? DaysOfWeek,
        int? DayOfMonth,
        string? CronExpression) : base(Route)
    {
        this.Title = Title;
        this.Description = Description;
        this.Frequency = Frequency;
        this.Interval = Interval;
        this.DaysOfWeek = DaysOfWeek;
        this.DayOfMonth = DayOfMonth;
        this.CronExpression = CronExpression;
    }

    public const string Route = "api/scheduled-jobs";
    public string Title { get; init; }
    public string Description { get; init; }
    public Frequency Frequency { get; init; }
    public int Interval { get; init; }
    public DayOfWeek[]? DaysOfWeek { get; init; }
    public int? DayOfMonth { get; init; }
    public string? CronExpression { get; init; }
    public DateTime AnchorDate { get; set; }
};