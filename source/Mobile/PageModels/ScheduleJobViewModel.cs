using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JobScheduleNotifications.Contracts;
using JobScheduleNotifications.Contracts.Customers;

namespace Mobile.PageModels;

public partial class ScheduleJobViewModel : ObservableObject
{
    private readonly IJobService _jobService;
    private readonly ICustomerService _customerService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<CustomerDto> customers = new();

    [ObservableProperty]
    private CustomerDto selectedCustomer;

    [ObservableProperty]
    private DateTime scheduledDate = DateTime.Now;

    [ObservableProperty]
    private TimeSpan scheduledTime = DateTime.Now.TimeOfDay;

    [ObservableProperty]
    private TimeSpan minimumTime = DateTime.Now.TimeOfDay;

    [ObservableProperty]
    private string jobDescription = string.Empty;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private string title = "Schedule Job";

    [ObservableProperty]
    private bool isBusy;

    public ScheduleJobViewModel(
        IJobService jobService, 
        ICustomerService customerService, 
        INavigationService navigationService, 
        CustomerDto selectedCustomer)
    {
        _jobService = jobService;
        _customerService = customerService;
        _navigationService = navigationService;
        SelectedCustomer = selectedCustomer;
    }

    partial void OnScheduledDateChanged(DateTime value)
    {
        var thing = SelectedCustomer;
        if (value.Date == DateTime.Now.Date)
        {
            MinimumTime = DateTime.Now.TimeOfDay;
        }
        else
        {
            MinimumTime = TimeSpan.Zero;
        }
    }

    [RelayCommand]
    private async Task LoadCustomers()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var customerList = await _customerService.GetCustomersAsync();
            Customers.Clear();
            foreach (var customer in customerList)
            {
                Customers.Add(customer);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "Failed to load customers", "OK");
            System.Diagnostics.Debug.WriteLine($"Load Customers Error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ScheduleJob()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            if (SelectedCustomer == null)
            {
                ErrorMessage = "Please select a customer";
                return;
            }

            if (string.IsNullOrWhiteSpace(JobDescription))
            {
                ErrorMessage = "Please enter a job description";
                return;
            }

            var scheduledDateTime = ScheduledDate.Date.Add(ScheduledTime);
            if (scheduledDateTime < DateTime.Now)
            {
                ErrorMessage = "Scheduled time must be in the future";
                return;
            }

            var job = new CreateJobDto
            {
                CustomerId = SelectedCustomer.Id,
                ScheduledDate = scheduledDateTime,
                Description = JobDescription
            };

            await _jobService.CreateJobAsync(job);
            await Shell.Current.DisplayAlert("Success", "Job scheduled successfully", "OK");
            await _navigationService.GoBackAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to schedule job";
            System.Diagnostics.Debug.WriteLine($"Schedule Job Error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}