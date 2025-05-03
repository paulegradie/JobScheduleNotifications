using Api.Business.Entities.Base;
using Api.ValueTypes;
using Server.Contracts.Dtos;

namespace Api.Business.Entities;

public class ScheduledJobDefinitionDomainModel : DomainModelBase<ScheduledJobDefinitionDto>
{
    public ScheduledJobDefinitionId ScheduledJobDefinitionId { get; set; }
    public CustomerId CustomerId { get; set; }
    public DateTime AnchorDate { get; set; }
    public RecurrencePatternDomainModel Pattern { get; set; }
    public List<JobOccurrenceDomainModel> JobOccurrences { get; set; } = new();

    public string Title { get; set; }
    public string Description { get; set; }

    public override ScheduledJobDefinitionDto ToDto()
    {
        return new ScheduledJobDefinitionDto(
            CustomerId,
            ScheduledJobDefinitionId,
            AnchorDate,
            Pattern.ToDto(),
            JobOccurrences.Select(o => o.ToDto()).ToList(),
            Title,
            Description);
    }
}