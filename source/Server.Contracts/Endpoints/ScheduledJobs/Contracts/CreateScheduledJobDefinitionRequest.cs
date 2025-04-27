using Api.ValueTypes;
using Api.ValueTypes.Enums;
using Server.Contracts.Common.Request;

namespace Server.Contracts.Endpoints.ScheduledJobs.Contracts;

public sealed record CreateScheduledJobDefinitionRequest(
    CustomerId Id,
    string Title,
    string Description,
    Frequency Frequency,
    int Interval,
    WeekDays[]? WeekDays,
    int? DayOfMonth,
    string? CronExpression,
    DateTime AnchorDate) : RequestBase(Route)
{
    public const string Route = $"api/customers/{CustomerIdSegmentParam}/jobs";

    public override ApiRoute GetApiRoute()
    {
        var route = base.GetApiRoute();
        route.AddRouteParam(CustomerIdSegmentParam, Id.ToString());
        return route;
    }
}
