using System.Runtime.CompilerServices;
using Server.Client.Endpoints;
using Server.Contracts.Client;
using Server.Contracts.Client.Endpoints.Auth;
using Server.Contracts.Client.Endpoints.Customers;
using Server.Contracts.Client.Endpoints.Home;
using Server.Contracts.Client.Endpoints.JobOccurence;
using Server.Contracts.Client.Endpoints.Reminders;
using Server.Contracts.Client.Endpoints.ScheduledJobs;

[assembly: InternalsVisibleTo("Mobile.Composition")]
[assembly: InternalsVisibleTo("IntegrationTests")]

namespace Server.Client;

internal class ServerClient : IServerClient
{
    public ServerClient(HttpClient client)
    {
        Http = client;
        Home = new HomeEndpoint(client);
        Customers = new CustomersEndpoint(client);
        Auth = new AuthEndpoint(client);
        ScheduledJobs = new ScheduledJobsEndpoint(client);
        JobOccurrences = new JobOccurrencesEndpoint(client);
        JobReminders = new JobRemindersEndpoint(client);
    }

    public IJobOccurrencesEndpoint JobOccurrences { get; init; }
    public IJobRemindersEndpoint JobReminders { get; init; }
    public HttpClient Http { get; set; }

    public IHomeEndpoint Home { get; init; }
    public ICustomersEndpoint Customers { get; init; }
    public IAuthenticationEndpoint Auth { get; init; }
    public IScheduledJobsEndpoint ScheduledJobs { get; init; }
}

internal class AuthClient : IAuthClient
{
    public AuthClient(HttpClient client)
    {
        Http = client;
        Auth = new AuthEndpoint(client);
    }

    public HttpClient Http { get; set; }
    public IAuthenticationEndpoint Auth { get; init; }
}