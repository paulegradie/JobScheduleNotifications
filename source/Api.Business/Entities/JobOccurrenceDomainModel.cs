using Api.Business.Entities.Base;
using Api.ValueTypes;

namespace Api.Business.Entities;

public class JobOccurrenceDomainModel : DomainModelBase<JobOccurrenceDto>
{
    public JobOccurrenceId Id { get; set; }
    public ScheduledJobDefinitionId ScheduledJobDefinitionId { get; set; }
    public DateTime OccurrenceDate { get; set; }

    // All reminders for this occurrence
    public List<JobReminderDomainModel> JobReminders { get; set; } = new();

    public override JobOccurrenceDto ToDto()
    {
        return new JobOccurrenceDto(
            Id,
            ScheduledJobDefinitionId,
            OccurrenceDate,
            JobReminders.Select(r => r.ToDto()).ToList()
        );
    }
}