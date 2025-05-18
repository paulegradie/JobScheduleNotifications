using Api.ValueTypes;

namespace Mobile.UI.Pages.Customers.ScheduledJobs.JobOccurrences;

public record CustomerJobAndOccurrenceIds(CustomerId CustomerId, ScheduledJobDefinitionId ScheduledJobDefinitionId, JobOccurrenceId JobOccurrenceId);