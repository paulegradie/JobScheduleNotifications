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
    public List<JobReminderDomainModel> JobReminders { get; set; } = [];


    public string Title { get; set; }
    public string Description { get; set; }

    public override ScheduledJobDefinitionDto ToDto()
    {
        var occDtos = JobOccurrences.Select(o =>
        {
            var occ = o.ToDto();
            return occ;
        }).ToList();
        var reminderDtos = JobReminders.Select(o => o.ToDto()).ToList();
        return new ScheduledJobDefinitionDto(
            CustomerId,
            ScheduledJobDefinitionId,
            AnchorDate,
            CronExpression,
            occDtos,
            Title,
            Description,
            DayOfMonth,
            reminderDtos);
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

    public JobReminderDomainModel CreateNewReminder()
    {
        return new JobReminderDomainModel(
            reminderDateTime: DateTime.Now,
            message: $"You have a job coming up: {Title} - {Description}",
            scheduledJobDefinitionId: ScheduledJobDefinitionId,
            isSent: false,
            sentDate: null);
    }
}