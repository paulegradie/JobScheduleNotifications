using Api.Business.Features.ScheduledJobs;
using Api.ValueTypes;

namespace Api.Business.Entities;

public class ScheduledJobDefinitionDomainModel
{
    public ScheduledJobDefinitionId Id { get; set; }
    public CustomerId CustomerId { get; set; }
    public DateTime AnchorDate { get; set; }
    public RecurrencePattern Pattern { get; set; }
    public List<JobOccurrenceDomainModel> JobOccurrences { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }
}