using Api.Business.Entities;

namespace Api.Business.Repositories;

public interface ISendNotificationOfUpcomingJobs
{
    Task SendNotification(ScheduledJobDefinitionDomainModel jobDefinitionDomainModel);
}