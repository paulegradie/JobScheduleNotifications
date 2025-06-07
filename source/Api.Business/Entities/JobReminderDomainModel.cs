using System.Diagnostics.CodeAnalysis;
using Api.Business.Entities.Base;
using Api.ValueTypes;
using Server.Contracts.Dtos;

namespace Api.Business.Entities;

public class JobReminderDomainModel : DomainModelBase<JobReminderDto>
{
    public JobReminderDomainModel(
        DateTime reminderDateTime,
        string message,
        ScheduledJobDefinitionId scheduledJobDefinitionId,
        bool isSent,
        DateTime? sentDate)
    {
        ReminderDateTime = reminderDateTime;
        Message = message;
        ScheduledJobDefinitionId = scheduledJobDefinitionId;
        IsSent = isSent;
        SentDate = sentDate;
    }

    public JobReminderId? JobReminderId { get; protected set; }

    public void SetJobReminderId(JobReminderId id)
    {
        JobReminderId = id;
    }

    public ScheduledJobDefinitionId ScheduledJobDefinitionId { get; protected set; }

    public void SetScheduledJobDefinitionId(ScheduledJobDefinitionId id)
    {
        ScheduledJobDefinitionId = id;
    }


    public DateTime ReminderDateTime { get; set; }


    public string Message { get; set; } = string.Empty;

    [MemberNotNullWhen(true, nameof(SentDate))]
    public bool IsSent { get; protected set; }

    public void SetIsSent()
    {
        IsSent = true;
        SentDate = DateTime.Now;
    }

    public DateTime? SentDate { get; protected set; }

    public override JobReminderDto ToDto()
    {
        if (JobReminderId is null)
        {
            throw new InvalidOperationException("Dtos are only valid when primary id is not null");
        }

        return new JobReminderDto
        {
            JobReminderId = JobReminderId.Value,
            ScheduledJobDefinitionId = ScheduledJobDefinitionId,
            ReminderDateTime = ReminderDateTime,
            Message = Message,
            IsSent = IsSent,
            SentDate = SentDate
        };
    }

    public override void FromDto(JobReminderDto dto)
    {
        JobReminderId = dto.JobReminderId;
        ScheduledJobDefinitionId = dto.ScheduledJobDefinitionId;
        ReminderDateTime = dto.ReminderDateTime;
        Message = dto.Message;
        IsSent = dto.IsSent;
        SentDate = dto.SentDate;
    }
}