﻿using Api.ValueTypes;
using Server.Contracts.Common.Request;

namespace Server.Contracts.Endpoints.Reminders.Contracts;

public sealed record UpdateJobReminderRequest(
    CustomerId CustomerId,
    ScheduledJobDefinitionId JobDefinitionId,
    JobReminderId ReminderId,
    DateTime ReminderDateTime,
    string Message)
    : RequestBase(Route)
{
    public const string Route = $"api/customers/{CustomerIdSegmentParam}/jobs/{JobDefinitionIdSegmentParam}/reminders/{JobReminderIdSegmentParam}";

    protected override ApiRoute GetApiRoute()
    {
        var route = base.GetApiRoute();
        route.AddRouteParam(CustomerIdSegmentParam, CustomerId.ToString());
        route.AddRouteParam(JobDefinitionIdSegmentParam, JobDefinitionId.ToString());
        route.AddRouteParam(JobReminderIdSegmentParam, ReminderId.ToString());
        return route;
    }
}