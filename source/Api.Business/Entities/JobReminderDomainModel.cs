using Api.Business.Entities.Base;
using Api.ValueTypes;
using Server.Contracts.Dtos;

namespace Api.Business.Entities;

public class JobReminderDomainModel : DomainModelBase<JobReminderDto>
{
    public JobReminderDomainModel()
    {
    }

    public JobReminderDomainModel(DateTime reminderDateTime, string message)
    {
        ReminderDateTime = reminderDateTime;
        Message = message;
    }

    public JobReminderId Id { get; set; }
    public JobOccurrenceId JobOccurrenceId { get; set; } // ← newly surfaced
    public ScheduledJobDefinitionId ScheduledJobDefinitionId { get; set; }
    public DateTime ReminderDateTime { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsSent { get; set; }
    public DateTime? SentDate { get; set; }

    public override JobReminderDto ToDto()
        => new()
        {
            Id = Id,
            ScheduledJobDefinitionId = ScheduledJobDefinitionId,
            ReminderTime = ReminderDateTime,
            Message = Message,
            IsSent = IsSent,
            SentDate = SentDate
        };

    public override void FromDto(JobReminderDto dto)
    {
        Id = dto.Id;
        ScheduledJobDefinitionId = dto.ScheduledJobDefinitionId;
        ReminderDateTime = dto.ReminderTime;
        Message = dto.Message;
        IsSent = dto.IsSent;
        SentDate = dto.SentDate;
    }
}