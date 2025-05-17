using Api.Infrastructure.DbTables.OrganizationModels;
using Api.ValueTypes;

namespace Api.Infrastructure.DbTables.Jobs;

public class ScheduledJobDefinition
{
    public ScheduledJobDefinitionId ScheduledJobDefinitionId { get; set; }
    public CustomerId CustomerId { get; set; }

    public virtual Customer Customer { get; set; }

    public DateTime AnchorDate { get; set; } // the very first run (or “start counting from”)

    public string Title { get; set; } = "";
    public string Description { get; set; } = "";

    public string CronExpression { get; set; } = "";
    public virtual ICollection<JobOccurrence> JobOccurrences { get; set; } = new List<JobOccurrence>();
}
