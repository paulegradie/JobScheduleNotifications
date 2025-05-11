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
        private readonly IJobService _jobService;
        private readonly ICustomerService _customerService;
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
            IJobService jobService,
            ICustomerService customerService,
            INavigationRepository navigation)
        {
            _jobService = jobService;
            _customerService = customerService;
            _navigation = navigation;
        }


        [RelayCommand]
        private async Task LoadScheduledJob(ScheduledJobDefinitionId scheduledJobDefinitionId)
        {
            await RunWithSpinner(async () =>
            {
                var result = await _jobService.GetJobAsync(SelectedCustomer.Id, scheduledJobDefinitionId);
                var customer = await _customerService.GetCustomerAsync(result.CustomerId);
                Title = result.Title;
                CustomerName = customer.FullName;
                AnchorDate = result.AnchorDate;
                Description = result.Description;
                Frequency = result.Pattern.Frequency;
                Interval = result.Pattern.Interval;
                DayOfMonth = result.Pattern.DayOfMonth;
                SelectedWeekDays = new ObservableCollection<WeekDay>(result.Pattern.WeekDays);
            });
        }
    }
}