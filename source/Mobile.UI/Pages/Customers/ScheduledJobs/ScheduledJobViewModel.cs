using System.Collections.ObjectModel;
using Api.ValueTypes;
using Api.ValueTypes.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.RepositoryAbstractions;
using Mobile.UI.Services;
using Server.Contracts.Dtos;

namespace Mobile.UI.Pages.Customers.ScheduledJobs
{
    public partial class ScheduledJobViewModel : Base.BaseViewModel
    {
        private readonly IJobRepository _jobRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly INavigationRepository _navigation;

        [ObservableProperty] private ObservableCollection<CustomerDto> _customers = new();

        [ObservableProperty] private CustomerDto _selectedCustomer;

        [ObservableProperty] private string _title = "Schedule Job";

        [ObservableProperty] private DateTime _anchorDate = DateTime.Now;

        [ObservableProperty] private Frequency _frequency = Frequency.Daily;

        [ObservableProperty] private int _interval = 1;

        [ObservableProperty] private ObservableCollection<WeekDay> _selectedWeekDays = new();

        [ObservableProperty] private int? _dayOfMonth;

        [ObservableProperty] private string _cronExpression;

        [ObservableProperty] private string _description = string.Empty;
        [ObservableProperty] private string _customerName = string.Empty;

        public ScheduledJobViewModel(
            IJobRepository jobRepository,
            ICustomerRepository customerRepository,
            INavigationRepository navigation)
        {
            _jobRepository = jobRepository;
            _customerRepository = customerRepository;
            _navigation = navigation;
        }


        [RelayCommand]
        private async Task LoadScheduledJob(ScheduledJobDefinitionId scheduledJobDefinitionId)
        {
            await RunWithSpinner(async () =>
            {
                var scheduledJobResult = await _jobRepository.GetJobByIdAsync(SelectedCustomer.Id, scheduledJobDefinitionId);
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
                    }
                }
            });
        }
    }
}