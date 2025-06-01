using Api.ValueTypes;
using Server.Contracts.Common.Request;

namespace Server.Contracts.Endpoints.Reminders.Contracts;

public record ListJobRemindersRequest(CustomerId CustomerId, ScheduledJobDefinitionId JobDefinitionId, bool? IsSent = null)
    : RequestBase(Route)
{
    public const string Route = $"api/customers/{CustomerIdSegmentParam}/jobs/{JobDefinitionIdSegmentParam}/reminders";

    protected override ApiRoute GetApiRoute()
    {
        var route = base.GetApiRoute();
        route.AddRouteParam(CustomerIdSegmentParam, CustomerId.ToString());
        route.AddRouteParam(JobDefinitionIdSegmentParam, JobDefinitionId.ToString());
        if (IsSent.HasValue)
        {
            route.AddQueryParam(JobOccurrenceIsSentQueryParam, IsSent?.ToString() ?? string.Empty);
        }
        return route;
    }
}