using Api.ValueTypes;
using Server.Contracts.Common.Request;

namespace Server.Contracts.Client.Endpoints.ScheduledJobs;

public record CreateScheduledJobRequest(CustomerId CustomerId) : RequestBase(Route)
{
    public const string Route = "api/scheduled-jobs";

    public DateTime DateTime { get; set; }
};