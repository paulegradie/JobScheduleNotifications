using System.Collections.ObjectModel;
using Api.ValueTypes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages.Base;
using Mobile.UI.Pages.Customers.ScheduledJobs.JobOccurrences;
using Mobile.UI.RepositoryAbstractions;
using Mobile.UI.Navigation;
using Server.Contracts.Dtos;

namespace Mobile.UI.Pages.Customers.ScheduledJobs;

/// <summary>
/// ViewModel for the Scheduled Job page.
///
/// NAVIGATION UPDATES:
/// This class has been updated to demonstrate the new type-safe navigation system.
/// Key improvements:
/// - NavigateToOccurrenceAsync: Now uses type-safe navigation with compile-time parameter validation
/// - NavigateToReminderAsync: Fixed to include ALL required parameters (was missing JobOccurrenceId and JobReminderId)
/// - Added demonstration methods showing different navigation patterns
/// - All navigation calls now include proper error handling and validation
///
/// The old dictionary-based navigation was error-prone and could cause runtime failures.
/// The new system ensures all required parameters are provided at compile time.
/// </summary>
public partial class ScheduledJobViewModel : BaseViewModel
{
    private readonly IJobRepository _jobRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IJobOccurrenceRepository _jobOccurrenceRepository;

    private const int occurrencePageSize = 3;
    private List<JobOccurrenceDto> _allOccurrences = new();
    private int _occurrenceCursor;

    [ObservableProperty] private ObservableCollection<JobOccurrenceDto> _jobOccurrences = new();

    [ObservableProperty] private ObservableCollection<JobReminderDto> _jobReminders = new();

    [ObservableProperty] private bool _hasMoreOccurrences;

    [ObservableProperty] private string _title = string.Empty;

    [ObservableProperty] private string _customerName = string.Empty;

    [ObservableProperty] private DateTime _anchorDate;

    [ObservableProperty] private string _description = string.Empty;

    [ObservableProperty] private string _cronExpression = string.Empty;

    private CustomerId CustomerId { get; set; }
    private ScheduledJobDefinitionId ScheduledJobDefinitionId { get; set; }

    public LoadParametersCustomerIdAndScheduleJobDefId LoadParametersCustomerIdAndScheduleJobDefId { get; set; }
    public JobOccurrenceId JobOccurrenceId { get; set; }
    private CustomerJobAndOccurrenceIds? CustomerJobAndOccurrenceIds { get; set; }

    public ScheduledJobViewModel(
        IJobRepository jobRepository,
        ICustomerRepository customerRepository,
        IJobOccurrenceRepository jobOccurrenceRepository)
    {
        _jobRepository = jobRepository;
        _customerRepository = customerRepository;
        _jobOccurrenceRepository = jobOccurrenceRepository;
        _anchorDate = DateTime.Now;
    }

    [RelayCommand]
    private async Task LoadScheduledJobAsync(LoadParametersCustomerIdAndScheduleJobDefId loadParametersCustomerIdAndScheduleJobDefId)
    {
        LoadParametersCustomerIdAndScheduleJobDefId = loadParametersCustomerIdAndScheduleJobDefId;
        await RunWithSpinner(async () =>
        {
            var scheduledJobResult = await _jobRepository.GetJobByIdAsync(
                loadParametersCustomerIdAndScheduleJobDefId.CustomerId, loadParametersCustomerIdAndScheduleJobDefId.ScheduledJobDefinitionId);
            if (!scheduledJobResult.IsSuccess) return;

            var scheduledJob = scheduledJobResult.Value;
            CustomerId = scheduledJob.CustomerId;
            ScheduledJobDefinitionId = scheduledJob.ScheduledJobDefinitionId;

            var customerResult = await _customerRepository.GetCustomerByIdAsync(CustomerId);
            if (!customerResult.IsSuccess) return;

            Title = scheduledJob.Title;
            CustomerName = customerResult.Value.FullName;
            AnchorDate = scheduledJob.AnchorDate;
            Description = scheduledJob.Description;
            CronExpression = scheduledJob.CronExpression;

            // setup occurrences paging
            _allOccurrences = scheduledJob.JobOccurrences
                .OrderByDescending(x => x.OccurrenceDate)
                .ToList();
            JobOccurrences.Clear();
            _occurrenceCursor = 0;
            HasMoreOccurrences = _allOccurrences.Count > occurrencePageSize;
            LoadMoreOccurrences();

            // load reminders
            JobReminders.Clear();
            foreach (var reminder in scheduledJob.JobReminders)
            {JobReminders.Add(reminder);}
        });
    }

    [RelayCommand(CanExecute = nameof(CanLoadMoreOccurrences))]
    private void LoadMoreOccurrences()
    {
        var nextBatch = _allOccurrences
            .Skip(_occurrenceCursor)
            .Take(occurrencePageSize)
            .ToList();

        foreach (var occ in nextBatch)
            JobOccurrences.Add(occ);

        _occurrenceCursor += nextBatch.Count;
        HasMoreOccurrences = _occurrenceCursor < _allOccurrences.Count;
        LoadMoreOccurrencesCommand.NotifyCanExecuteChanged();

        // Ensure UI updates
        OnPropertyChanged(nameof(JobOccurrences));
        OnPropertyChanged(nameof(HasMoreOccurrences));
    }

    private bool CanLoadMoreOccurrences() => HasMoreOccurrences;

    [RelayCommand]
    private async Task AddOccurrenceAsync()
    {
        await RunWithSpinner(async () =>
        {
            var results = await _jobOccurrenceRepository
                .CreateNewOccurrence(
                    LoadParametersCustomerIdAndScheduleJobDefId.CustomerId,
                    LoadParametersCustomerIdAndScheduleJobDefId.ScheduledJobDefinitionId,
                    CancellationToken.None);

            if (!results.IsSuccess) return;

            // insert new occurrence and reload pages
            _allOccurrences.Insert(0, results.Value.Occurrence);
            JobOccurrences.Clear();
            _occurrenceCursor = 0;
            HasMoreOccurrences = _allOccurrences.Count > occurrencePageSize;
            LoadMoreOccurrences();
            OnPropertyChanged(nameof(JobOccurrences));
            OnPropertyChanged(nameof(HasMoreOccurrences));
        });
    }

    [RelayCommand]
    private async Task NavigateToOccurrenceAsync(JobOccurrenceId jobOccurrenceId)
    {
        CustomerJobAndOccurrenceIds = new CustomerJobAndOccurrenceIds(
            LoadParametersCustomerIdAndScheduleJobDefId.CustomerId,
            LoadParametersCustomerIdAndScheduleJobDefId.ScheduledJobDefinitionId,
            jobOccurrenceId);

        await RunWithSpinner(async () =>
        {
            try
            {
                // NEW: Type-safe navigation with compile-time parameter validation
                await Navigation.NavigateToJobOccurrenceAsync(
                    LoadParametersCustomerIdAndScheduleJobDefId.CustomerId,
                    LoadParametersCustomerIdAndScheduleJobDefId.ScheduledJobDefinitionId,
                    jobOccurrenceId);
            }
            catch (ArgumentException ex)
            {
                // Handle validation errors gracefully
                await ShowAlertAsync("Navigation Error", ex.Message);
            }
        });
    }

    /// <summary>
    /// Navigate to a specific reminder. This method demonstrates the type-safe navigation
    /// and shows how all required parameters must be provided.
    /// </summary>
    [RelayCommand]
    private async Task NavigateToReminderAsync(JobReminderDto? reminder = null)
    {
        var ids = CustomerJobAndOccurrenceIds;
        if (ids == null)
        {
            await ShowErrorAsync("No job occurrence selected. Please select a job occurrence first.");
            return;
        }

        // If no specific reminder is provided, let user select one
        if (reminder == null)
        {
            if (!JobReminders.Any())
            {
                await ShowErrorAsync("This job has no reminders to view.");
                return;
            }

            // For demonstration, we'll use the first reminder
            // In a real app, you might show a selection dialog
            reminder = JobReminders.First();

            var shouldProceed = await ShowConfirmationAsync(
                "Select Reminder",
                $"Navigate to reminder scheduled for {reminder.ReminderDateTime:MMM d, yyyy h:mm tt}?");

            if (!shouldProceed) return;
        }

        try
        {
            await Navigation.NavigateToJobReminderAsync(
                ids.CustomerId,
                ids.ScheduledJobDefinitionId,
                ids.JobOccurrenceId,
                reminder.JobReminderId);
        }
        catch (ArgumentException ex)
        {
            await ShowAlertAsync("Navigation Error", ex.Message);
        }
    }

    /// <summary>
    /// Example: Navigate to create an invoice for a job occurrence
    /// This demonstrates the type-safe navigation with multiple parameters including strings
    /// </summary>
    [RelayCommand]
    private async Task NavigateToCreateInvoiceAsync(JobOccurrenceDto? occurrence = null)
    {
        var ids = CustomerJobAndOccurrenceIds;
        if (ids == null)
        {
            await ShowAlertAsync("Navigation Error",
                "No job occurrence selected. Please select a job occurrence first.");
            return;
        }

        // Use the provided occurrence or find it from the current occurrence ID
        var targetOccurrence = occurrence ?? _allOccurrences.FirstOrDefault(o => o.JobOccurrenceId == ids.JobOccurrenceId);
        if (targetOccurrence == null)
        {
            await ShowErrorAsync("Could not find job occurrence details.");
            return;
        }

        try
        {
            // NEW: Type-safe navigation with extension method for clean syntax
            await Navigation.NavigateToCreateInvoiceAsync(
                ids.CustomerId,
                ids.ScheduledJobDefinitionId,
                ids.JobOccurrenceId,
                Description); // Using the job description from the current view model
        }
        catch (ArgumentException ex)
        {
            await ShowErrorAsync($"Navigation Error - {ex.Message}");
        }
    }

    /// <summary>
    /// Example: Navigate back to the customer's job list
    /// This demonstrates navigation with a single parameter
    /// </summary>
    [RelayCommand]
    private async Task NavigateToCustomerJobsAsync()
    {
        try
        {
            // NEW: Type-safe navigation using extension method
            await Navigation.NavigateToCustomerJobsAsync(LoadParametersCustomerIdAndScheduleJobDefId.CustomerId);
        }
        catch (ArgumentException ex)
        {
            await ShowErrorAsync($"Navigation Error - {ex.Message}");
        }
    }

    /// <summary>
    /// Example: Generic type-safe navigation
    /// This demonstrates the most flexible approach when page type is known at compile time
    /// </summary>
    [RelayCommand]
    private async Task NavigateToCustomerViewGenericAsync()
    {
        try
        {
            // NEW: Generic type-safe navigation - most flexible approach
            await Navigation.NavigateToAsync<CustomerViewPage, CustomerParameters>(
                new CustomerParameters(LoadParametersCustomerIdAndScheduleJobDefId.CustomerId));
        }
        catch (ArgumentException ex)
        {
            await ShowErrorAsync($"Navigation Error - {ex.Message}");
        }
    }
}