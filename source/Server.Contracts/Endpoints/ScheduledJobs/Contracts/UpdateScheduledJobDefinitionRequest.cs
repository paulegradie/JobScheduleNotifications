using Api.ValueTypes;
using Server.Contracts.Common.Request;

namespace Server.Contracts.Endpoints.ScheduledJobs.Contracts;

public sealed record UpdateScheduledJobDefinitionRequest : RequestBase
{
    public const string Route = $"api/customers/{CustomerIdSegmentParam}/jobs/{JobDefinitionIdSegmentParam}/next";

    protected override ApiRoute GetApiRoute()
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
    public string CronExpression { get; init; }
    public DateTime AnchorDate { get; set; }

    public UpdateScheduledJobDefinitionRequest() : base(Route)
    {
    }

    private UpdateScheduledJobDefinitionRequest(
        CustomerId customerId,
        ScheduledJobDefinitionId jobDefinitionId,
        DateTime anchorDate,
        string title,
        string description,
        string cronExpression)
        : base(Route)
    {
        CustomerId = customerId;
        JobDefinitionId = jobDefinitionId;
        Title = title;
        Description = description;
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
                _anchorDate ?? DateTime.UtcNow,
                _title,
                _description,
                _cronExpression);
    }
}