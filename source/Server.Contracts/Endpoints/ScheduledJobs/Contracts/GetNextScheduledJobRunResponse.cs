namespace Server.Contracts.Endpoints.ScheduledJobs.Contracts;

public sealed record GetNextScheduledJobRunResponse(DateTime NextRun);