using Server.Contracts.Client.Endpoints.Auth;
using Server.Contracts.Client.Endpoints.Customers;
using Server.Contracts.Client.Endpoints.Home;
using Server.Contracts.Client.Endpoints.JobOccurence;
using Server.Contracts.Client.Endpoints.Reminders;
using Server.Contracts.Client.Endpoints.ScheduledJobs;

namespace Server.Contracts.Client;

public interface IServerClient
{
    public IHomeEndpoint Home { get; init; }
    public ICustomersEndpoint Customers { get; init; }
    public IAuthenticationEndpoint Auth { get; init; }
    public IScheduledJobsEndpoint ScheduledJobs { get; init; }
    public IJobOccurrencesEndpoint JobOccurrences { get; init; }
    public IJobRemindersEndpoint JobReminders { get; init; }


    public HttpClient Http { get; set; }
}

public interface IAuthClient
{
    public IAuthenticationEndpoint Auth { get; init; }
    public HttpClient Http { get; set; }
}