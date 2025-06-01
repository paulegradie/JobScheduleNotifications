using Api.ValueTypes;
using Server.Contracts.Common.Request;

namespace Server.Contracts.Endpoints.ScheduledJobs.Contracts;

public sealed record GetNextScheduledJobRunRequest(CustomerId CustomerId, ScheduledJobDefinitionId ScheduledJobDefinitionId) : RequestBase(Route)
{
    public const string Route = $"api/customers/{CustomerIdSegmentParam}/jobs/{JobDefinitionIdSegmentParam}/next";

    protected override ApiRoute GetApiRoute()
    {
        var route = base.GetApiRoute();
        route.AddRouteParam(CustomerIdSegmentParam, CustomerId.ToString());
        route.AddRouteParam(JobDefinitionIdSegmentParam, ScheduledJobDefinitionId.ToString());
        return route;
    }
}