using Server.Contracts.Endpoints.Auth;
using Server.Contracts.Endpoints.Customers;
using Server.Contracts.Endpoints.Home;
using Server.Contracts.Endpoints.Invoices;
using Server.Contracts.Endpoints.JobOccurence;
using Server.Contracts.Endpoints.JobPhotos;
using Server.Contracts.Endpoints.Reminders;
using Server.Contracts.Endpoints.ScheduledJobs;

namespace Server.Contracts;

public interface IServerClient
{
    public IHomeEndpoint Home { get; init; }
    public ICustomersEndpoint Customers { get; init; }
    public IAuthenticationEndpoint Auth { get; init; }
    public IScheduledJobsEndpoint ScheduledJobs { get; init; }
    public IJobOccurrencesEndpoint JobOccurrences { get; init; }
    public IJobRemindersEndpoint JobReminders { get; init; }
    public IInvoiceEndpoint Invoices { get; init; }
    public IJobCompletedPhotosEndpoint JobCompletedPhotos { get; init; }
    public IDashboardEndpoint DashboardEndpoint { get; init; }


    public HttpClient Http { get; set; }
}

public interface IAuthClient
{
    public IAuthenticationEndpoint Auth { get; init; }
    public HttpClient Http { get; set; }
}