using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api.ValueTypes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages.Base;
using Mobile.UI.Pages.Customers.ScheduledJobs.JobOccurrences;
using Mobile.UI.Pages.Customers.ScheduledJobs.JobReminders;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts.Dtos;

namespace Mobile.UI.Pages.Customers.ScheduledJobs;

public partial class ScheduledJobViewModel : BaseViewModel
{
    private readonly IJobRepository _jobRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly INavigationRepository _navigation;
    private readonly IJobOccurrenceRepository _jobOccurrenceRepository;

    private const int occurrencePageSize = 3;
    private List<JobOccurrenceDto> _allOccurrences = new();
    private int _occurrenceCursor;

    [ObservableProperty]
    private ObservableCollection<JobOccurrenceDto> _jobOccurrences = new();

    [ObservableProperty]
    private ObservableCollection<JobReminderDto> _jobReminders = new();

    [ObservableProperty]
    private bool _hasMoreOccurrences;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _customerName = string.Empty;

    [ObservableProperty]
    private DateTime _anchorDate;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _cronExpression = string.Empty;

    private CustomerId CustomerId { get; set; }
    private ScheduledJobDefinitionId ScheduledJobDefinitionId { get; set; }

    public Details Details { get; set; }
    public JobOccurrenceId JobOccurrenceId { get; set; }
    private CustomerJobAndOccurrenceIds? CustomerJobAndOccurrenceIds { get; set; }

    public ScheduledJobViewModel(
        IJobRepository jobRepository,
        ICustomerRepository customerRepository,
        INavigationRepository navigation,
        IJobOccurrenceRepository jobOccurrenceRepository)
    {
        _jobRepository = jobRepository;
        _customerRepository = customerRepository;
        _navigation = navigation;
        _jobOccurrenceRepository = jobOccurrenceRepository;
        _anchorDate = DateTime.Now;
    }

    [RelayCommand]
    private async Task LoadScheduledJobAsync(Details details)
    {
        Details = details;
        await RunWithSpinner(async () =>
        {
            var scheduledJobResult = await _jobRepository.GetJobByIdAsync(
                details.CustomerId, details.ScheduledJobDefinitionId);
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
                JobReminders.Add(reminder);
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
                    Details.CustomerId,
                    Details.ScheduledJobDefinitionId,
                    CancellationToken.None);

            if (!results.IsSuccess) return;

            // insert new occurrence and reload pages
            _allOccurrences.Insert(0, results.Value.Occurrence);
            JobOccurrences.Clear();
            _occurrenceCursor = 0;
            HasMoreOccurrences = _allOccurrences.Count > occurrencePageSize;
            LoadMoreOccurrences();

            // Explicitly notify that properties have changed
            OnPropertyChanged(nameof(JobOccurrences));
            OnPropertyChanged(nameof(HasMoreOccurrences));
        });
    }

    [RelayCommand]
    private async Task NavigateToOccurrenceAsync(JobOccurrenceId jobOccurrenceId)
    {
        CustomerJobAndOccurrenceIds = new CustomerJobAndOccurrenceIds(
            Details.CustomerId,
            Details.ScheduledJobDefinitionId,
            jobOccurrenceId);

        await RunWithSpinner(async () =>
        {
            await _navigation.GoToAsync(
                nameof(ViewJobOccurrencePage),
                new Dictionary<string, object>
                {
                    ["CustomerId"] = Details.CustomerId.Value.ToString(),
                    ["ScheduledJobDefinitionId"] = Details.ScheduledJobDefinitionId.Value.ToString(),
                    ["JobOccurrenceId"] = jobOccurrenceId.Value.ToString()
                });
        });
    }

    [RelayCommand]
    private async Task NavigateToReminderAsync()
    {
        var ids = CustomerJobAndOccurrenceIds;
        if (ids == null) return;

        await _navigation.GoToAsync(
            nameof(JobReminderPage),
            new Dictionary<string, object>
            {
                ["CustomerId"] = ids.CustomerId.Value.ToString(),
                ["ScheduledJobDefinitionId"] = ids.ScheduledJobDefinitionId.Value.ToString(),
            });
    }
}