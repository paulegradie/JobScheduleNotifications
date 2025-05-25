using System.Collections.ObjectModel;
using Api.ValueTypes;
using Api.ValueTypes.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages.Base;
using Mobile.UI.Pages.Customers.ScheduledJobs.JobOccurrences;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts.Dtos;

namespace Mobile.UI.Pages.Customers.ScheduledJobs;

public partial class ScheduledJobViewModel : BaseViewModel
{
    private readonly IJobRepository _jobRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly INavigationRepository _navigation;
    private readonly IJobOccurrenceRepository _jobOccurrenceRepository;

    [ObservableProperty] private ObservableCollection<CustomerDto> _customers = [];

    [ObservableProperty] private CustomerDto _selectedCustomer;

    [ObservableProperty] private string _title = "Schedule Job";

    [ObservableProperty] private DateTime _anchorDate = DateTime.Now;

    [ObservableProperty] private Frequency _frequency = Frequency.Daily;

    [ObservableProperty] private int _interval = 1;

    [ObservableProperty] private ObservableCollection<WeekDay> _selectedWeekDays = [];

    [ObservableProperty] private int? _dayOfMonth;

    [ObservableProperty] private string _cronExpression;

    [ObservableProperty] private string _description = string.Empty;
    [ObservableProperty] private string _customerName = string.Empty;
    [ObservableProperty] private ObservableCollection<JobOccurrenceDto> _jobOccurrences = [];

    private CustomerId CustomerId { get; set; }
    private ScheduledJobDefinitionId ScheduledJobDefinitionId { get; set; }
    

    public Details Details { get; set; }

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
    }

    public JobOccurrenceId JobOccurrenceId { get; set; }

    [RelayCommand]
    private async Task NavigateToOccurrenceAsync(JobOccurrenceId jobOccurrenceId)
    {
        await RunWithSpinner(async () =>
        {
            await _navigation.GoToAsync(nameof(ViewJobOccurrencePage), new Dictionary<string, object>
            {
                ["JobOccurrenceId"] = jobOccurrenceId.Value.ToString(),
                ["CustomerId"] = Details.CustomerId.Value.ToString(),
                ["ScheduledJobDefinitionId"] = Details.ScheduledJobDefinitionId.Value.ToString()
            });
        });
    }

    [RelayCommand]
    private async Task LoadScheduledJob(Details details)
    {
        Details = details;
        await RunWithSpinner(async () =>
        {
            var scheduledJobResult = await _jobRepository.GetJobByIdAsync(details.CustomerId, details.ScheduledJobDefinitionId);
            if (scheduledJobResult.IsSuccess)
            {
                var scheduledJob = scheduledJobResult.Value;
                var customerId = scheduledJobResult.Value.CustomerId;
                var customerResult = await _customerRepository.GetCustomerByIdAsync(customerId);
                if (customerResult.IsSuccess)
                {
                    var customer = customerResult.Value;
                    Title = scheduledJob.Title;
                    CustomerName = customer.FullName;
                    AnchorDate = scheduledJob.AnchorDate;
                    Description = scheduledJob.Description;
                    CronExpression = scheduledJob.CronExpression;
                    JobOccurrences = new ObservableCollection<JobOccurrenceDto>(scheduledJob.JobOccurrences);
                }
            }
        });
    }

    [RelayCommand]
    private async Task AddOccurrenceAsync()
    {
        var details = Details;
        await RunWithSpinner(async () =>
        {
            var results = await _jobOccurrenceRepository
                .CreateNewOccurrence(details.CustomerId, details.ScheduledJobDefinitionId, CancellationToken.None);

            if (results.IsSuccess)
            {
                var jobOccurrenceDto = results.Value.Occurrence;
                JobOccurrences.Add(jobOccurrenceDto);
                OnPropertyChanged(nameof(JobOccurrences));
            }
        });
    }
}