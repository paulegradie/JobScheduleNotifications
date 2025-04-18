using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using JobScheduleNotifications.Contracts;
using JobScheduleNotifications.Contracts.Customers;
using JobScheduleNotifications.Mobile.Services;

namespace JobScheduleNotifications.Mobile.ViewModels;

public class ScheduleJobViewModel : BaseViewModel
{
    private readonly IJobService _jobService;
    private readonly ICustomerService _customerService;
    private readonly INavigationService _navigationService;

    private ObservableCollection<CustomerDto> _customers;
    private CustomerDto _selectedCustomer;

    private DateTime _scheduledDate = DateTime.Now;
    private TimeSpan _scheduledTime = DateTime.Now.TimeOfDay;
    private string _jobDescription = string.Empty;

    private string _errorMessage = string.Empty;


    public ScheduleJobViewModel(IJobService jobService, ICustomerService customerService, INavigationService navigationService, CustomerDto selectedCustomer)
    {
        _jobService = jobService;
        _customerService = customerService;
        _navigationService = navigationService;
        _selectedCustomer = selectedCustomer;
        Title = "Schedule Job";
        _customers = new ObservableCollection<CustomerDto>();
        
        LoadCustomersCommand = new Command(async () => await LoadCustomersAsync());
        ScheduleJobCommand = new Command(async () => await ScheduleJobAsync());
    }

#pragma warning disable CA2227
    public ObservableCollection<CustomerDto> Customers
#pragma warning restore CA2227
    {
        get => _customers;
        set => SetProperty(ref _customers, value);
    }

    public CustomerDto SelectedCustomer
    {
        get => _selectedCustomer;
        set => SetProperty(ref _selectedCustomer, value);
    }

    public DateTime ScheduledDate
    {
        get => _scheduledDate;
        set
        {
            if (SetProperty(ref _scheduledDate, value))
            {
                // Update minimum time when date changes
                if (value.Date == DateTime.Now.Date)
                {
                    MinimumTime = DateTime.Now.TimeOfDay;
                }
                else
                {
                    MinimumTime = TimeSpan.Zero;
                }
            }
        }
    }

    public TimeSpan ScheduledTime
    {
        get => _scheduledTime;
        set => SetProperty(ref _scheduledTime, value);
    }

    public TimeSpan MinimumTime { get; private set; } = DateTime.Now.TimeOfDay;

    public string JobDescription
    {
        get => _jobDescription;
        set => SetProperty(ref _jobDescription, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public ICommand LoadCustomersCommand { get; }
    public ICommand ScheduleJobCommand { get; }

    private async Task LoadCustomersAsync()
    {
        if (IsBusy) return;



        try
        {
            SetBusy(true);
            var customers = await _customerService.GetCustomersAsync();
            Customers.Clear();
            foreach (var customer in customers)
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
            SetBusy(false);
        }
    }

    private async Task ScheduleJobAsync()
    {
        if (IsBusy) return;

        try
        {
            SetBusy(true);
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
            SetBusy(false);
        }
    }
} 