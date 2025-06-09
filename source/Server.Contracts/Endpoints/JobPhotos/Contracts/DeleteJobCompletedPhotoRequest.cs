using Api.ValueTypes;
using Server.Contracts.Common.Request;

namespace Server.Contracts.Endpoints.JobPhotos.Contracts;

public sealed record DeleteJobCompletedPhotoRequest(
    CustomerId CustomerId,
    ScheduledJobDefinitionId ScheduledJobDefinitionId,
    JobOccurrenceId JobOccurrenceId,
    JobCompletedPhotoId JobCompletedPhotoId
) : RequestBase(Route)
{
    public const string Route = $"customers/{CustomerIdSegmentParam}/jobs/{JobDefinitionIdSegmentParam}/occurrences/{JobOccurenceIdSegmentParam}/jobCompletedPhotos/{PhotoIdSegmentParam}";
    protected override ApiRoute GetApiRoute()
    {
        var route = base.GetApiRoute();
        route.AddRouteParam(CustomerIdSegmentParam, CustomerId.ToString());
        route.AddRouteParam(JobDefinitionIdSegmentParam, ScheduledJobDefinitionId.ToString());
        route.AddRouteParam(JobOccurenceIdSegmentParam, JobOccurrenceId.ToString());
        route.AddRouteParam(PhotoIdSegmentParam, JobCompletedPhotoId.ToString());
        return route;
    }
}