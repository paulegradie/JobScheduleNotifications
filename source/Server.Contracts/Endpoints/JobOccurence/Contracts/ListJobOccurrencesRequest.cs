using Api.ValueTypes;
using Server.Contracts.Common.Request;

namespace Server.Contracts.Endpoints.JobOccurence.Contracts;

public sealed record ListJobOccurrencesRequest(
    CustomerId CustomerId,
    ScheduledJobDefinitionId JobDefinitionId)
    : RequestBase(Route)
{
    public const string Route =
        $"api/customers/{CustomerIdSegmentParam}/jobs/{JobDefinitionIdSegmentParam}/occurrences";

    protected override ApiRoute GetApiRoute()
    {
        var route = base.GetApiRoute();
        route.AddRouteParam(CustomerIdSegmentParam, CustomerId.ToString());
        route.AddRouteParam(JobDefinitionIdSegmentParam, JobDefinitionId.ToString());
        return route;
    }
}