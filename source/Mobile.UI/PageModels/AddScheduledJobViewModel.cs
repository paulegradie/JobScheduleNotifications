using System.Collections.ObjectModel;
using Api.ValueTypes.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.RepositoryAbstractions;
using Mobile.UI.Services;
using Server.Contracts.Dtos;

namespace Mobile.UI.PageModels;

 public partial class AddScheduledJobViewModel : ObservableObject
    {
        private readonly IJobService _jobService;
        private readonly ICustomerService _customerService;
        private readonly INavigationRepository _navigation;

        [ObservableProperty]
        private ObservableCollection<CustomerDto> _customers = new();

        [ObservableProperty]
        private CustomerDto _selectedCustomer;

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private DateTime _anchorDate = DateTime.Now;

        [ObservableProperty]
        private Frequency _frequency = Frequency.Daily;

        [ObservableProperty]
        private int _interval = 1;

        [ObservableProperty]
        private ObservableCollection<WeekDays> _selectedWeekDays = new();

        [ObservableProperty]
        private int? _dayOfMonth;

        [ObservableProperty]
        private string _cronExpression = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        public AddScheduledJobViewModel(
            IJobService jobService,
            ICustomerService customerService,
            INavigationRepository navigation)
        {
            _jobService = jobService;
            _customerService = customerService;
            _navigation = navigation;
        }

        [RelayCommand]
        private async Task LoadCustomersAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var list = await _customerService.GetCustomersAsync();
                Customers.Clear();
                foreach (var c in list)
                    Customers.Add(c);

                // pick the first if none selected yet
                SelectedCustomer ??= Customers.FirstOrDefault();
            }
            catch
            {
                ErrorMessage = "Unable to load customers.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CreateJobAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            ErrorMessage = string.Empty;

            if (SelectedCustomer == null)
            {
                ErrorMessage = "Select a customer.";
            }
            else if (string.IsNullOrWhiteSpace(Title))
            {
                ErrorMessage = "Enter a title.";
            }
            else if (string.IsNullOrWhiteSpace(Description))
            {
                ErrorMessage = "Enter a description.";
            }
            else
            {
                try
                {
                    var dto = new CreateScheduledJobDefinitionDto
                    {
                        CustomerId       = SelectedCustomer.Id,
                        Title            = Title,
                        Description      = Description,
                        AnchorDate       = AnchorDate,
                        Frequency        = Frequency,
                        Interval         = Interval,
                        WeekDays         = SelectedWeekDays.ToArray(),
                        DayOfMonth       = DayOfMonth,
                        CronExpression   = CronExpression
                    };

                    await _jobService.CreateJobAsync(dto);
                    await Shell.Current.DisplayAlert("Success", "Job scheduled!", "OK");
                    await _navigation.GoBackAsync();
                }
                catch
                {
                    ErrorMessage = "Failed to schedule job.";
                }
            }

            IsBusy = false;
        }
    }