namespace Api.Business.Entities;

public class JobOccurrenceDomainModel
{
    public DateTime OccurrenceDate { get; set; }
    public List<JobReminderDomainModel> JobReminders { get; set; } = new();
}