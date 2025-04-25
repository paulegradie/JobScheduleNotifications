using Api.ValueTypes;
using Server.Contracts.Common.Request;

namespace Server.Contracts.Client.Endpoints.ScheduledJobs;

public sealed record GetScheduledJobDefinitionByIdResponse(ScheduledJobDefinitionDto JobDefinition);

public sealed record GetScheduledJobDefinitionByIdRequest(CustomerId CustomerId, ScheduledJobDefinitionId JobDefinitionId) : RequestBase(Route)
{
    public const string Route = $"api/customers/{CustomerIdSegmentParam}/jobs/{JobDefinitionIdSegmentParam}";


    public override ApiRoute GetApiRoute()
    {
        var route = base.GetApiRoute();
        route.AddRouteParam(CustomerIdSegmentParam, CustomerId.ToString());
        route.AddRouteParam(JobDefinitionIdSegmentParam, JobDefinitionId.ToString());
        return route;
    }
}