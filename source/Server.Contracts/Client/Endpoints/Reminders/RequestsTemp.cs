using Api.ValueTypes;
using Server.Contracts.Common.Request;
using Server.Contracts.Dtos;

namespace Server.Contracts.Client.Endpoints.Reminders;

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

ListJobRemindersResponse

public record GetJobReminderByIdRequest(
    CustomerId CustomerId,
    ScheduledJobDefinitionId JobDefinitionId,
    JobReminderId ReminderId)
    : RequestBase(Route)
{
    public const string Route = $"api/customers/{CustomerIdSegmentParam}/jobs/{JobDefinitionIdSegmentParam}/reminders/{JobReminderIdSegmentParam}";

    public override ApiRoute GetApiRoute()
    {
        var route = base.GetApiRoute();
        route.AddRouteParam(CustomerIdSegmentParam, CustomerId.ToString());
        route.AddRouteParam(JobDefinitionIdSegmentParam, JobDefinitionId.ToString());
        route.AddRouteParam(JobReminderIdSegmentParam, ReminderId.ToString());
        return route;
    }
}

public sealed record GetJobReminderByIdResponse(JobReminderDto JobReminderDto);

public sealed record CreateJobReminderRequest(
    CustomerId CustomerId,
    ScheduledJobDefinitionId JobDefinitionId,
    DateTime ReminderDateTime,
    string Message)
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

public sealed record CreateJobReminderResponse(JobReminderDto JobReminderDto);

public sealed record UpdateJobReminderRequest(
    CustomerId CustomerId,
    ScheduledJobDefinitionId JobDefinitionId,
    JobReminderId ReminderId,
    DateTime ReminderDateTime,
    string Message)
    : RequestBase(Route)
{
    public const string Route = $"api/customers/{CustomerIdSegmentParam}/jobs/{JobDefinitionIdSegmentParam}/reminders/{JobReminderIdSegmentParam}";

    public override ApiRoute GetApiRoute()
    {
        var route = base.GetApiRoute();
        route.AddRouteParam(CustomerIdSegmentParam, CustomerId.ToString());
        route.AddRouteParam(JobDefinitionIdSegmentParam, JobDefinitionId.ToString());
        route.AddRouteParam(JobReminderIdSegmentParam, ReminderId.ToString());
        return route;
    }
}

public sealed record UpdateJobReminderResponse(JobReminderDto JobReminderDto);

public record DeleteJobReminderRequest(
    CustomerId CustomerId,
    ScheduledJobDefinitionId JobDefinitionId,
    JobReminderId ReminderId)
    : RequestBase(Route)
{
    public const string Route = $"api/customers/{CustomerIdSegmentParam}/jobs/{JobDefinitionIdSegmentParam}/reminders/{JobReminderIdSegmentParam}";

    public override ApiRoute GetApiRoute()
    {
        var route = base.GetApiRoute();
        route.AddRouteParam(CustomerIdSegmentParam, CustomerId.ToString());
        route.AddRouteParam(JobDefinitionIdSegmentParam, JobDefinitionId.ToString());
        route.AddRouteParam(JobReminderIdSegmentParam, ReminderId.ToString());
        return route;
    }
}

public record AcknowledgeJobReminderRequest(
    CustomerId CustomerId,
    ScheduledJobDefinitionId JobDefinitionId,
    JobReminderId ReminderId)
    : RequestBase(Route)
{
    public const string Route = $"api/customers/{CustomerIdSegmentParam}/jobs/{JobDefinitionIdSegmentParam}/reminders/{JobReminderIdSegmentParam}/ack";

    public override ApiRoute GetApiRoute()
    {
        var route = base.GetApiRoute();
        route.AddRouteParam(CustomerIdSegmentParam, CustomerId.ToString());
        route.AddRouteParam(JobDefinitionIdSegmentParam, JobDefinitionId.ToString());
        route.AddRouteParam(JobReminderIdSegmentParam, ReminderId.ToString());
        return route;
    }
}

public sealed record AcknowledgeJobReminderResponse(JobReminderDto JobReminderDto);