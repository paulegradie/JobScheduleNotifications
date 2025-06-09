using Api.Business.Repositories;
using Api.Business.Repositories.Internal;
using Api.Controllers.Base;
using Api.Infrastructure.DbTables.Jobs;
using Api.Infrastructure.DbTables.OrganizationModels;
using Api.Infrastructure.Services;
using Api.ValueTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Endpoints.JobPhotos.Contracts;

namespace Api.Controllers;

public class DashboardController : BaseApiController
{
    private readonly ICrudRepository<Customer, CustomerId> _customerCrudRepository;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IUserSettingsRepository _userSettingsRepository;
    private readonly UserManager<ApplicationUserRecord> _userManager;

    public DashboardController(
        ICrudRepository<Customer, CustomerId> customerCrudRepository,
        ICurrentUserContext currentUserContext,
        IUserSettingsRepository userSettingsRepository,
        UserManager<ApplicationUserRecord> userManager)
    {
        _customerCrudRepository = customerCrudRepository;
        _currentUserContext = currentUserContext;
        _userSettingsRepository = userSettingsRepository;
        _userManager = userManager;
    }

    [HttpGet(GetDashboardContentRequest.Route)]
    public async Task<ActionResult<DashboardResponse>> GetDashboardContent()
    {
        var allCustomers = (await _customerCrudRepository.GetAllAsync()).ToList();
        var currentUser = await _userSettingsRepository.GetCurrentUserSettings();

        var customerIds = allCustomers.SelectMany(x => x.ScheduledJobDefinitions).ToList();
        var totalCompletedJobs = allCustomers
            .Sum(x =>
                x.ScheduledJobDefinitions
                    .Sum(y =>
                        y.JobOccurrences.Count(z =>
                            z.MarkedAsCompleted)));

        var pendingJobs = allCustomers
            .Sum(x =>
                x.ScheduledJobDefinitions
                    .Sum(y =>
                        y.JobOccurrences.Count(z =>
                            z is { MarkedAsCompleted: false, JobOccurrenceStatus: JobOccurrenceStatus.NotStarted })));

        var numJobsInProgress = allCustomers
            .Sum(x =>
                x.ScheduledJobDefinitions
                    .Sum(y =>
                        y.JobOccurrences.Count(z =>
                            z is { MarkedAsCompleted: false, JobOccurrenceStatus: JobOccurrenceStatus.InProgress })));

        var numUnInvoicedJobs = allCustomers
            .Sum(x =>
                x.ScheduledJobDefinitions
                    .Sum(y =>
                        y.JobOccurrences.Count(z =>
                            z is { MarkedAsCompleted: true, JobOccurrenceStatus: JobOccurrenceStatus.Completed, JobOccurenceInvoiceStatus: JobOccurenceInvoiceStatus.Issued })));
            
            
        var dto = new DashboardDto
        {
            Customers = allCustomers.Select(x => x.ToDto()).ToList(),
            BusinessName = currentUser.BusinessName,
            CurrentUser = currentUser.UserName,
            PendingJobs = pendingJobs,
            TotalJobsAcrossCustomers = customerIds.Count,
            TotalCompletedJobs = totalCompletedJobs,
            NumJobsInProgress = numJobsInProgress,
            UnInvoicedJobs = numUnInvoicedJobs
        };
        return new ActionResult<DashboardResponse>(new DashboardResponse(dto));
    }
}