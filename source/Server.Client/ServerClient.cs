using System.Runtime.CompilerServices;
using Server.Client.Endpoints;
using Server.Contracts;
using Server.Contracts.Endpoints.Auth;
using Server.Contracts.Endpoints.Customers;
using Server.Contracts.Endpoints.Home;
using Server.Contracts.Endpoints.Invoices;
using Server.Contracts.Endpoints.JobOccurence;
using Server.Contracts.Endpoints.JobPhotos;
using Server.Contracts.Endpoints.OrganizationSettings;
using Server.Contracts.Endpoints.Reminders;
using Server.Contracts.Endpoints.ScheduledJobs;

[assembly: InternalsVisibleTo("Mobile.Composition")]
[assembly: InternalsVisibleTo("Tests.IntegrationTests")]

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
        Invoices = new InvoiceEndpoint(client);
        JobCompletedPhotos = new JobCompletedPhotosEndpoint(client);
        DashboardEndpoint = new DashboardEndpoint(client);
        OrganizationSettings = new OrganizationSettingsEndpoint(client);
    }

    public IJobOccurrencesEndpoint JobOccurrences { get; init; }
    public IJobRemindersEndpoint JobReminders { get; init; }
    public IJobCompletedPhotosEndpoint JobCompletedPhotos { get; init; }
    public HttpClient Http { get; set; }

    public IDashboardEndpoint DashboardEndpoint { get; init; }
    public IHomeEndpoint Home { get; init; }
    public ICustomersEndpoint Customers { get; init; }
    public IAuthenticationEndpoint Auth { get; init; }
    public IScheduledJobsEndpoint ScheduledJobs { get; init; }
    public IInvoiceEndpoint Invoices { get; init; }
    public IOrganizationSettingsEndpoint OrganizationSettings { get; init; }
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