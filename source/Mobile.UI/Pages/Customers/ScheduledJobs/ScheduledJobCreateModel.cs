using System.Collections.ObjectModel;
using Api.ValueTypes;
using Api.ValueTypes.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.RepositoryAbstractions;
using Mobile.UI.Services;
using Server.Contracts.Dtos;

namespace Mobile.UI.Pages.Customers.ScheduledJobs;

public partial class ScheduledJobCreateModel : BaseViewModel
{
    private readonly IJobService _jobService;
    private readonly ICustomerService _customerService;
    private readonly INavigationRepository _navigation;

    [ObservableProperty] private ObservableCollection<CustomerDto> _customers = new();

    [ObservableProperty] private CustomerDto _selectedCustomer;

    [ObservableProperty] private string _title = string.Empty;

    [ObservableProperty] private string _description = string.Empty;

    [ObservableProperty] private DateTime _anchorDate = DateTime.Now;

    [ObservableProperty] private Frequency _frequency = Frequency.Daily;

    [ObservableProperty] private int _interval = 1;

    [ObservableProperty] private ObservableCollection<WeekDay> _selectedWeekDays = new();

    [ObservableProperty] private int? _dayOfMonth;

    [ObservableProperty] private string _cronExpression = string.Empty;


    public ScheduledJobCreateModel(
        IJobService jobService,
        ICustomerService customerService,
        INavigationRepository navigation)
    {
        _jobService = jobService;
        _customerService = customerService;
        _navigation = navigation;
    }


    [RelayCommand]
    private async Task LoadCustomersAsync(string customerId) // from the route!
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var customerDtos = (await _customerService.GetCustomersAsync()).ToList();
            Customers.Clear();
            foreach (var customerDto in customerDtos)
            {
                Customers.Add(customerDto);
            }

            SelectedCustomer = customerDtos.FirstOrDefault(x => x.Id.Value.ToString() == customerId) ?? customerDtos.First();

            // TODO: Load up the rest of the properties too?
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
                    CustomerId = SelectedCustomer.Id,
                    Title = Title,
                    Description = Description,
                    AnchorDate = AnchorDate,
                    Frequency = Frequency,
                    Interval = Interval,
                    WeekDays = null,
                    DayOfMonth = DayOfMonth,
                    CronExpression = CronExpression
                };

                await _jobService.CreateJobAsync(dto);
                await Shell.Current.DisplayAlert("Success", "Job scheduled!", "OK");
                await _navigation.GoToAsync(nameof(CustomerListPage));
            }
            catch
            {
                ErrorMessage = "Failed to schedule job.";
            }
        }

        IsBusy = false;
    }
}