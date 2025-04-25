using Api.Business.Entities.Base;
using Api.Business.Features.ScheduledJobs;
using Api.ValueTypes;
using Server.Contracts.Client.Endpoints.ScheduledJobs;

namespace Api.Business.Entities;

public class ScheduledJobDefinitionDomainModel : DomainModelBase<ScheduledJobDefinitionDto>
{
    public ScheduledJobDefinitionId Id { get; set; }
    public CustomerId CustomerId { get; set; }
    public DateTime AnchorDate { get; set; }
    public RecurrencePattern Pattern { get; set; }
    public List<JobOccurrenceDomainModel> JobOccurrences { get; set; } = new();

    public string Title { get; set; }
    public string Description { get; set; }

    public override ScheduledJobDefinitionDto ToDto()
    {
        return new ScheduledJobDefinitionDto(Id, Title, Description, AnchorDate);
    }
}