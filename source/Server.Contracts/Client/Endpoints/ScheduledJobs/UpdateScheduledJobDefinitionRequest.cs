using Api.ValueTypes;
using Api.ValueTypes.Enums;
using Server.Contracts.Common.Request;

namespace Server.Contracts.Client.Endpoints.ScheduledJobs;

public sealed record UpdateScheduledJobDefinitionResponse(ScheduledJobDefinitionDto ScheduledJobDefinitionDto);

public sealed record UpdateScheduledJobDefinitionRequest : RequestBase
{
    public const string Route = $"api/customers/{CustomerIdSegmentParam}/jobs/{JobDefinitionIdSegmentParam}/next";


    public override ApiRoute GetApiRoute()
    {
        var route = base.GetApiRoute();
        route.AddRouteParam(CustomerIdSegmentParam, CustomerId.ToString());
        route.AddRouteParam(JobDefinitionIdSegmentParam, JobDefinitionId.ToString());
        return route;
    }

    public CustomerId CustomerId { get; init; }
    public ScheduledJobDefinitionId JobDefinitionId { get; init; }

    public string? Title { get; init; }
    public string? Description { get; init; }
    public Frequency? Frequency { get; init; }
    public int? Interval { get; init; }
    public DayOfWeek[]? DaysOfWeek { get; init; }
    public int? DayOfMonth { get; init; }
    public string? CronExpression { get; init; }

    private UpdateScheduledJobDefinitionRequest(
        CustomerId customerId,
        ScheduledJobDefinitionId jobDefinitionId,
        string? title = null,
        string? description = null,
        Frequency? frequency = null,
        int? interval = null,
        DayOfWeek[]? daysOfWeek = null,
        int? dayOfMonth = null,
        string? cronExpression = null)
        : base(Route)
    {
        CustomerId = customerId;
        JobDefinitionId = jobDefinitionId;
        Title = title;
        Description = description;
        Frequency = frequency;
        Interval = interval;
        DaysOfWeek = daysOfWeek;
        DayOfMonth = dayOfMonth;
        CronExpression = cronExpression;
    }

    public static Builder CreateBuilder(
        CustomerId customerId,
        ScheduledJobDefinitionId jobDefinitionId) =>
        new(customerId, jobDefinitionId);

    public sealed class Builder
    {
        private readonly CustomerId _customerId;
        private readonly ScheduledJobDefinitionId _jobDefinitionId;
        private string? _title;
        private string? _description;
        private Frequency? _frequency;
        private int? _interval;
        private DayOfWeek[]? _daysOfWeek;
        private int? _dayOfMonth;
        private string? _cronExpression;

        internal Builder(
            CustomerId customerId,
            ScheduledJobDefinitionId jobDefinitionId)
        {
            _customerId = customerId;
            _jobDefinitionId = jobDefinitionId;
        }

        public Builder WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public Builder WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public Builder WithFrequency(Frequency frequency)
        {
            _frequency = frequency;
            return this;
        }

        public Builder WithInterval(int interval)
        {
            _interval = interval;
            return this;
        }

        public Builder WithDaysOfWeek(DayOfWeek[] daysOfWeek)
        {
            _daysOfWeek = daysOfWeek;
            return this;
        }

        public Builder WithDayOfMonth(int dayOfMonth)
        {
            _dayOfMonth = dayOfMonth;
            return this;
        }

        public Builder WithCronExpression(string cronExpression)
        {
            _cronExpression = cronExpression;
            return this;
        }

        public UpdateScheduledJobDefinitionRequest Build() =>
            new UpdateScheduledJobDefinitionRequest(
                _customerId,
                _jobDefinitionId,
                _title,
                _description,
                _frequency,
                _interval,
                _daysOfWeek,
                _dayOfMonth,
                _cronExpression);
    }
}