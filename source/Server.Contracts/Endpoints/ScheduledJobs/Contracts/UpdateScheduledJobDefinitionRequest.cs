using Api.ValueTypes;
using Api.ValueTypes.Enums;
using Server.Contracts.Common.Request;

namespace Server.Contracts.Endpoints.ScheduledJobs.Contracts;

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
    public WeekDay[]? WeekDays { get; init; }
    public int? DayOfMonth { get; init; }
    public string? CronExpression { get; init; }
    public DateTime? AnchorDate { get; set; }

    public UpdateScheduledJobDefinitionRequest() : base(Route)
    {
    }

    private UpdateScheduledJobDefinitionRequest(
        CustomerId customerId,
        ScheduledJobDefinitionId jobDefinitionId,
        string? title = null,
        string? description = null,
        Frequency? frequency = null,
        int? interval = null,
        WeekDay[]? weekDays = null,
        int? dayOfMonth = null,
        string? cronExpression = null,
        DateTime? anchorDate = null)
        : base(Route)
    {
        CustomerId = customerId;
        JobDefinitionId = jobDefinitionId;
        Title = title;
        Description = description;
        Frequency = frequency;
        Interval = interval;
        WeekDays = weekDays;
        DayOfMonth = dayOfMonth;
        CronExpression = cronExpression;
        AnchorDate = anchorDate;
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
        private WeekDay[]? _daysOfWeek;
        private int? _dayOfMonth;
        private string? _cronExpression;
        private DateTime? _anchorDate;

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

        public Builder WithFrequency(Frequency? frequency)
        {
            _frequency = frequency;
            return this;
        }

        public Builder WithInterval(int? interval)
        {
            _interval = interval;
            return this;
        }

        public Builder WithWeekDays(WeekDay[] daysOfWeek)
        {
            _daysOfWeek = daysOfWeek;
            return this;
        }

        public Builder WithDayOfMonth(int? dayOfMonth)
        {
            _dayOfMonth = dayOfMonth;
            return this;
        }

        public Builder WithCronExpression(string cronExpression)
        {
            _cronExpression = cronExpression;
            return this;
        }

        public Builder WithAnchorDate(DateTime? anchorDate)
        {
            _anchorDate = anchorDate;
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
                _cronExpression,
                _anchorDate);
    }
}