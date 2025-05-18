using Api.ValueTypes;

namespace Mobile.UI.Pages.Customers.ScheduledJobs.JobOccurrences;

public record CustomerJobAndOccurrenceIds
{
    public CustomerJobAndOccurrenceIds(CustomerId CustomerId, ScheduledJobDefinitionId ScheduledJobDefinitionId, JobOccurrenceId JobOccurrenceId)
    {
        this.CustomerId = CustomerId;
        this.ScheduledJobDefinitionId = ScheduledJobDefinitionId;
        this.JobOccurrenceId = JobOccurrenceId;
    }

    public CustomerId CustomerId { get; init; }
    public ScheduledJobDefinitionId ScheduledJobDefinitionId { get; init; }
    public JobOccurrenceId JobOccurrenceId { get; init; }

    public void Deconstruct(out CustomerId CustomerId, out ScheduledJobDefinitionId ScheduledJobDefinitionId, out JobOccurrenceId JobOccurrenceId)
    {
        CustomerId = this.CustomerId;
        ScheduledJobDefinitionId = this.ScheduledJobDefinitionId;
        JobOccurrenceId = this.JobOccurrenceId;
    }
}