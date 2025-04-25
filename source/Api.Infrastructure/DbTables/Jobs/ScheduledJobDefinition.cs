using Api.Business.Features.ScheduledJobs;
using Api.Infrastructure.DbTables.OrganizationModels;
using Api.ValueTypes;

namespace Api.Infrastructure.DbTables.Jobs;

public class ScheduledJobDefinition
{
    public ScheduledJobDefinitionId Id { get; set; }
    public CustomerId CustomerId { get; set; }

    public virtual Customer Customer { get; set; }

    // the very first run (or “start counting from”)
    public DateTime AnchorDate { get; set; }

    public string Title { get; set; } = "";
    public string Description { get; set; } = "";

    public RecurrencePattern Pattern { get; set; } = null!;
    public virtual ICollection<JobOccurrence> JobOccurrences { get; set;  } = new List<JobOccurrence>();
}