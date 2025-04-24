using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.Core.Domain;
using Mobile.Core.Repositories;
using Mobile.Core.Services;
using Server.Contracts.Client.Endpoints.Customers.Contracts;
using Server.Contracts.Dtos;

namespace Mobile.UI.PageModels;

public partial class ScheduleJobViewModel : ObservableObject
{
    private readonly IJobRepository _jobRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly INavigationUtility _navigationUtility;

    [ObservableProperty] private ObservableCollection<ServiceRecipient> _customers = new();

    [ObservableProperty] private CustomerDto _selectedCustomer;

    [ObservableProperty] private DateTime _scheduledDate = DateTime.Now;

    [ObservableProperty] private TimeSpan _scheduledTime = DateTime.Now.TimeOfDay;

    [ObservableProperty] private TimeSpan _minimumTime = DateTime.Now.TimeOfDay;

    [ObservableProperty] private string _jobDescription = string.Empty;

    [ObservableProperty] private string _errorMessage = string.Empty;

    [ObservableProperty] private string _title = "Schedule Job";

    [ObservableProperty] private bool _isBusy;

    public ScheduleJobViewModel(
        IJobRepository jobRepository,
        ICustomerRepository customerRepository,
        INavigationUtility navigationUtility,
        CustomerDto selectedCustomer)
    {
        _jobRepository = jobRepository;
        _customerRepository = customerRepository;
        _navigationUtility = navigationUtility;
        SelectedCustomer = selectedCustomer;
    }

    partial void OnScheduledDateChanged(DateTime value)
    {
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
            var customerList = await _customerRepository.GetCustomers();
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

            await _jobRepository.CreateJobAsync(job);
            await Shell.Current.DisplayAlert("Success", "Job scheduled successfully", "OK");
            await _navigationUtility.GoBackAsync();
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