using Server.Contracts.Dtos;

namespace Server.Contracts.Endpoints.JobPhotos.Contracts;

public sealed class DashboardDto
{
    public List<CustomerDto> Customers { get; set; }
    public int TotalJobsAcrossCustomers { get; set; }
    public int PendingJobs { get; set; }
    public int TotalCompletedJobs { get; set; }
    public string BusinessName { get; set; }
    public string CurrentUser { get; set; }
    public int NumJobsInProgress { get; set; }
    public int UnInvoicedJobs { get; set; }
}