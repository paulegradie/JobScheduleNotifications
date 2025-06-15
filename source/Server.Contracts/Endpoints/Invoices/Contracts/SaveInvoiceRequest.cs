using Api.ValueTypes;
using Server.Contracts.Common.Request;

namespace Server.Contracts.Endpoints.Invoices.Contracts;

public record SaveInvoiceRequest(
    CustomerId CustomerId,
    ScheduledJobDefinitionId ScheduledJobDefinitionId,
    JobOccurrenceId JobOccurrenceId,
    InvoiceItem[] InvoiceItems,
    Stream InvoiceStream,
    string FileName
) : RequestBase(Route)
{
    public const string Route = $"api/invoices/{CustomerIdSegmentParam}/jobs/{JobDefinitionIdSegmentParam}/occurences/{JobOccurenceIdSegmentParam}/save";

    protected override ApiRoute GetApiRoute()
    {
        var route = base.GetApiRoute();
        route.AddRouteParam(CustomerIdSegmentParam, CustomerId.ToString());
        route.AddRouteParam(JobDefinitionIdSegmentParam, ScheduledJobDefinitionId.ToString());
        route.AddRouteParam(JobOccurenceIdSegmentParam, JobOccurrenceId.ToString());
        return route;
    }
}

public record InvoiceItem(string ItemNumber, string Description, decimal Price);