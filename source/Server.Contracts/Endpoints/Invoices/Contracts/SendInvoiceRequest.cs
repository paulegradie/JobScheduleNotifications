﻿using Api.ValueTypes;
using Server.Contracts.Common.Request;

namespace Server.Contracts.Endpoints.Invoices.Contracts;

public record SendInvoiceRequest(
    CustomerId CustomerId,
    ScheduledJobDefinitionId ScheduledJobDefinitionId,
    JobOccurrenceId JobOccurrenceId,
    InvoiceId InvoiceId
) : RequestBase(Route)
{
    public const string Route = $"api/invoices/{CustomerIdSegmentParam}/jobs/{JobDefinitionIdSegmentParam}/occurences/{JobOccurenceIdSegmentParam}/send/{InvoiceIdSegmentParam}";

    protected override ApiRoute GetApiRoute()
    {
        var route = base.GetApiRoute();
        route.AddRouteParam(CustomerIdSegmentParam, CustomerId.ToString());
        route.AddRouteParam(JobDefinitionIdSegmentParam, ScheduledJobDefinitionId.ToString());
        route.AddRouteParam(JobOccurenceIdSegmentParam, JobOccurrenceId.ToString());
        route.AddRouteParam(InvoiceIdSegmentParam, InvoiceId.ToString());
        return route;
    }
}