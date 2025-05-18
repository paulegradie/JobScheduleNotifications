using Api.Business.Entities.Base;
using Api.ValueTypes;
using Server.Contracts.Dtos;

namespace Api.Business.Entities;

public class ScheduledJobDefinitionDomainModel : DomainModelBase<ScheduledJobDefinitionDto>
{
    public ScheduledJobDefinitionId ScheduledJobDefinitionId { get; set; }
    public CustomerId CustomerId { get; set; }
    public DateTime AnchorDate { get; set; }
    public string CronExpression { get; set; }
    public int? DayOfMonth { get; set; }
    public List<JobOccurrenceDomainModel> JobOccurrences { get; set; } = [];

    public string Title { get; set; }
    public string Description { get; set; }

    public override ScheduledJobDefinitionDto ToDto()
    {
        return new ScheduledJobDefinitionDto(
            CustomerId,
            ScheduledJobDefinitionId,
            AnchorDate,
            CronExpression,
            JobOccurrences.Select(o => o.ToDto()).ToList(),
            Title,
            Description,
            DayOfMonth);
    }

    public override void FromDto(ScheduledJobDefinitionDto dto)
    {
        ScheduledJobDefinitionId = dto.ScheduledJobDefinitionId;
        CustomerId = dto.CustomerId;
        AnchorDate = dto.AnchorDate;
        CronExpression = dto.CronExpression;
        DayOfMonth = dto.DayOfMonth;
        JobOccurrences = dto.JobOccurrences
            .Select(o =>
            {
                var m = new JobOccurrenceDomainModel();
                m.FromDto(o);
                return m;
            })
            .ToList();
        Title = dto.Title;
        Description = dto.Description;
    }
}