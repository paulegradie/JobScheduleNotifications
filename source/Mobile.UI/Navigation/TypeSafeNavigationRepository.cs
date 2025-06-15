using Mobile.UI.Pages;
using Mobile.UI.Pages.Customers;
using Mobile.UI.Pages.Customers.ScheduledJobs;
using Mobile.UI.Pages.Customers.ScheduledJobs.JobOccurrences;
using Mobile.UI.Pages.Customers.ScheduledJobs.JobReminders;
using Mobile.UI.RepositoryAbstractions;

namespace Mobile.UI.Navigation;

/// <summary>
/// Implementation of type-safe navigation that wraps the existing INavigationRepository
/// </summary>
public class TypeSafeNavigationRepository : ITypeSafeNavigationRepository
{
    private readonly INavigationRepository _navigationRepository;

    // Page name mappings - centralized to avoid magic strings
    private static readonly Dictionary<Type, string> PageNames = new()
    {
        { typeof(LandingPage), nameof(LandingPage) },
        { typeof(CustomerListPage), nameof(CustomerListPage) },
        { typeof(CustomerViewPage), nameof(CustomerViewPage) },
        { typeof(CustomerEditPage), nameof(CustomerEditPage) },
        { typeof(CustomerCreatePage), nameof(CustomerCreatePage) },
        { typeof(ScheduledJobListPage), nameof(ScheduledJobListPage) },
        { typeof(ScheduledJobCreatePage), nameof(ScheduledJobCreatePage) },
        { typeof(ScheduledJobViewPage), nameof(ScheduledJobViewPage) },
        { typeof(ScheduledJobEditPage), nameof(ScheduledJobEditPage) },
        { typeof(ViewJobOccurrencePage), nameof(ViewJobOccurrencePage) },
        { typeof(JobReminderPage), nameof(JobReminderPage) },
        { typeof(InvoiceCreatePage), nameof(InvoiceCreatePage) },
    };

    public TypeSafeNavigationRepository(INavigationRepository navigationRepository)
    {
        _navigationRepository = navigationRepository;
    }

    public Task GoBackAsync() => _navigationRepository.GoBackAsync();

    public async Task NavigateToAsync<TPage, TParameters>(TParameters parameters)
        where TPage : class
        where TParameters : INavigationParameters
    {
        // Validate parameters before navigation
        parameters.Validate();

        // Get page name from type
        if (!PageNames.TryGetValue(typeof(TPage), out var pageName))
        {
            throw new InvalidOperationException($"Page type {typeof(TPage).Name} is not registered for navigation");
        }

        // Navigate using the underlying repository
        await _navigationRepository.GoToAsync(pageName, parameters.ToDictionary());
    }

    // Specific navigation methods with clear contracts
    public Task NavigateToCustomerCreateAsync() =>
        NavigateToAsync<CustomerCreatePage, CustomerListParameters>(new CustomerListParameters());

    public Task NavigateToLandingPageAsync() =>
        NavigateToAsync<LandingPage, CustomerListParameters>(new CustomerListParameters());

    public Task NavigateToCustomerListAsync() =>
        NavigateToAsync<CustomerListPage, CustomerListParameters>(new CustomerListParameters());

    public Task NavigateToCustomerViewAsync(CustomerParameters parameters) =>
        NavigateToAsync<CustomerViewPage, CustomerParameters>(parameters);

    public Task NavigateToCustomerEditAsync(CustomerParameters parameters) =>
        NavigateToAsync<CustomerEditPage, CustomerParameters>(parameters);

    public Task NavigateToScheduledJobListAsync(ScheduledJobListParameters parameters) =>
        NavigateToAsync<ScheduledJobListPage, ScheduledJobListParameters>(parameters);

    public Task NavigateToScheduledJobCreateAsync(CustomerParameters parameters) =>
        NavigateToAsync<ScheduledJobCreatePage, CustomerParameters>(parameters);

    public Task NavigateToScheduledJobViewAsync(ScheduledJobParameters parameters) =>
        NavigateToAsync<ScheduledJobViewPage, ScheduledJobParameters>(parameters);

    public Task NavigateToScheduledJobEditAsync(ScheduledJobParameters parameters) =>
        NavigateToAsync<ScheduledJobEditPage, ScheduledJobParameters>(parameters);

    public Task NavigateToJobOccurrenceAsync(JobOccurrenceParameters parameters) =>
        NavigateToAsync<ViewJobOccurrencePage, JobOccurrenceParameters>(parameters);

    public Task NavigateToJobReminderAsync(JobReminderParameters parameters) =>
        NavigateToAsync<JobReminderPage, JobReminderParameters>(parameters);

    public Task NavigateToInvoiceCreateAsync(InvoiceCreateParameters parameters) =>
        NavigateToAsync<InvoiceCreatePage, InvoiceCreateParameters>(parameters);
}