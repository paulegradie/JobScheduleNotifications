using Api.ValueTypes;
using Server.Contracts.Common.Request;

namespace Server.Contracts.Endpoints.Reminders.Contracts;

public record SendInvoiceRequest(
    CustomerId CustomerId,
    ScheduledJobDefinitionId ScheduledJobDefinitionId,
    JobOccurrenceId JobOccurrenceId,
    Stream PdfStream,
    string FileName
) : RequestBase(Route)
{
    public const string Route = $"api/invoices/{CustomerIdSegmentParam}/jobs/{JobDefinitionIdSegmentParam}/reminders/{JobOccurenceIdSegmentParam}/send";


    public override ApiRoute GetApiRoute()
    {
        var route = base.GetApiRoute();
        route.AddRouteParam(CustomerIdSegmentParam, CustomerId.ToString());
        route.AddRouteParam(JobDefinitionIdSegmentParam, ScheduledJobDefinitionId.ToString());
        route.AddRouteParam(JobOccurenceIdSegmentParam, JobOccurrenceId.ToString());
        return route;
    }
}