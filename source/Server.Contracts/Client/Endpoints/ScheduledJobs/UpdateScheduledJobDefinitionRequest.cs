using Api.ValueTypes;
using Api.ValueTypes.Enums;
using Server.Contracts.Common.Request;

namespace Server.Contracts.Client.Endpoints.ScheduledJobs;

public sealed record UpdateScheduledJobDefinitionResponse(ScheduledJobDefinitionDto ScheduledJobDefinitionDto);

public sealed record UpdateScheduledJobDefinitionRequest(
    CustomerId CustomerId,
    ScheduledJobDefinitionId ScheduledJobDefinitionId,
    string Title,
    string Description,
    Frequency Frequency,
    int Interval,
    DayOfWeek[]? DaysOfWeek,
    int? DayOfMonth,
    string? CronExpression
) : RequestBase(Route)
{
    public const string Route = $"api/customers/{CustomerIdSegmentParam}/jobs/{JobDefinitionIdSegmentParam}/next";

    public override ApiRoute GetApiRoute()
    {
        var route = base.GetApiRoute();
        route.AddRouteParam(CustomerIdSegmentParam, CustomerId.ToString());
        route.AddRouteParam(JobDefinitionIdSegmentParam, ScheduledJobDefinitionId.ToString());
        return route;
    }
}