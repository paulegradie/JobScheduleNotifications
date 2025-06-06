﻿using Api.ValueTypes;
using Server.Contracts.Common.Request;

namespace Server.Contracts.Endpoints.JobOccurence.Contracts;

public sealed record GetJobOccurrenceByIdRequest(
    CustomerId CustomerId,
    ScheduledJobDefinitionId JobDefinitionId,
    JobOccurrenceId JobOccurrenceId)
    : RequestBase(Route)
{
    public const string Route =
        $"api/customers/{CustomerIdSegmentParam}/jobs/{JobDefinitionIdSegmentParam}/occurrences/{JobOccurenceIdSegmentParam}";

    protected override ApiRoute GetApiRoute()
    {
        var route = base.GetApiRoute();
        route.AddRouteParam(CustomerIdSegmentParam, CustomerId.ToString());
        route.AddRouteParam(JobDefinitionIdSegmentParam, JobDefinitionId.ToString());
        route.AddRouteParam(JobOccurenceIdSegmentParam, JobOccurrenceId.ToString());
        return route;
    }
}