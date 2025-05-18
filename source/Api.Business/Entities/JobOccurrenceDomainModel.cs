using Api.Business.Entities.Base;
using Api.ValueTypes;
using Server.Contracts.Dtos;

namespace Api.Business.Entities;

public class JobOccurrenceDomainModel : DomainModelBase<JobOccurrenceDto>
{
    public JobOccurrenceId Id { get; set; }
    public ScheduledJobDefinitionId ScheduledJobDefinitionId { get; set; }
    public CustomerId CustomerId { get; set; }
    public DateTime OccurrenceDate { get; set; }
    public bool MarkedAsComplete { get; set; }

    /// <summary>
    /// When this occurrence was marked complete, or null if not done yet.
    /// </summary>
    public DateTime? CompletedDate { get; set; }

    public string JobDescription { get; set; }

    /// <summary>
    /// Title of the parent Scheduled Job Definition.
    /// </summary>
    public string JobTitle { get; set; } = string.Empty;

    // All reminders for this occurrence
    public List<JobReminderDomainModel> JobReminders { get; set; } = new();

    public override JobOccurrenceDto ToDto()
    {
        return new JobOccurrenceDto(
            JobOccurrenceId: Id,
            ScheduledJobDefinitionId: ScheduledJobDefinitionId,
            CustomerId: CustomerId,
            OccurrenceDate: OccurrenceDate,
            CompletedDate: CompletedDate,
            JobTitle: JobTitle,
            JobDescription: JobDescription,
            MarkedAsCompleted: MarkedAsComplete,
            JobReminders: JobReminders.Select(r => r.ToDto()).ToList()
        );
    }

    /// <summary>
    /// Optional: helper to load from a DTO back into this domain model.
    /// </summary>
    public override void FromDto(JobOccurrenceDto dto)
    {
        Id = dto.JobOccurrenceId;
        ScheduledJobDefinitionId = dto.ScheduledJobDefinitionId;
        CustomerId = dto.CustomerId;
        OccurrenceDate = dto.OccurrenceDate;
        CompletedDate = dto.CompletedDate;
        JobTitle = dto.JobTitle;
        JobDescription = dto.JobDescription;
        MarkedAsComplete = dto.MarkedAsCompleted;
        JobReminders = dto.JobReminders
            .Select(r =>
            {
                var m = new JobReminderDomainModel();
                m.FromDto(r);
                return m;
            })
            .ToList();
    }
}