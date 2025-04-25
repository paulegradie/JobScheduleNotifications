namespace Server.Contracts.Client.Endpoints.ScheduledJobs.Contracts;

public sealed record GetNextScheduledJobRunResponse(DateTime NextRun);