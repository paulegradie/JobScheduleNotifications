using Api.ValueTypes;
using Server.Contracts.Common.Request;

namespace Server.Contracts.Endpoints.JobPhotos.Contracts;

public sealed record UploadJobCompletedPhotoRequest(
    CustomerId CustomerId,
    ScheduledJobDefinitionId ScheduledJobDefinitionId,
    JobOccurrenceId JobOccurrenceId,
    Stream PhotoStream,
    string FileName
) : RequestBase(Route)
{
    public const string Route = $"api/photos/{CustomerIdSegmentParam}/jobs/{JobDefinitionIdSegmentParam}/occurrences/{JobOccurenceIdSegmentParam}/upload";

    protected override ApiRoute GetApiRoute()
    {
        var route = base.GetApiRoute();
        route.AddRouteParam(CustomerIdSegmentParam, CustomerId.ToString());
        route.AddRouteParam(JobDefinitionIdSegmentParam, ScheduledJobDefinitionId.ToString());
        route.AddRouteParam(JobOccurenceIdSegmentParam, JobOccurrenceId.ToString());
        return route;
    }
}