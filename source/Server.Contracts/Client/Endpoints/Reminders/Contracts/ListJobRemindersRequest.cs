using Api.ValueTypes;
using Server.Contracts.Common.Request;

namespace Server.Contracts.Client.Endpoints.Reminders.Contracts;

public record ListJobRemindersRequest(CustomerId CustomerId, ScheduledJobDefinitionId JobDefinitionId)
    : RequestBase(Route)
{
    public const string Route = $"api/customers/{CustomerIdSegmentParam}/jobs/{JobDefinitionIdSegmentParam}/reminders";

    public override ApiRoute GetApiRoute()
    {
        var route = base.GetApiRoute();
        route.AddRouteParam(CustomerIdSegmentParam, CustomerId.ToString());
        route.AddRouteParam(JobDefinitionIdSegmentParam, JobDefinitionId.ToString());
        return route;
    }
}