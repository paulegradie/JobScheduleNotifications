using Api.Business.Entities.Base;
using Api.ValueTypes;
using Server.Contracts.Dtos;

namespace Api.Business.Entities;

public class JobReminderDomainModel : DomainModelBase<JobReminderDto>
{
    public JobReminderId Id { get; set; }
    public JobOccurrenceId JobOccurrenceId { get; set; }            // ← newly surfaced
    public ScheduledJobDefinitionId ScheduledJobDefinitionId { get; set; }
    public DateTime ReminderDateTime { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsSent { get; set; }
    public DateTime? SentDate { get; set; }

    public override JobReminderDto ToDto()
        => new JobReminderDto
        {
            Id                       = Id,
            ScheduledJobDefinitionId = ScheduledJobDefinitionId,
            ReminderTime             = ReminderDateTime,
            Message                  = Message,
            IsSent                   = IsSent,
            SentDate                 = SentDate
        };
}