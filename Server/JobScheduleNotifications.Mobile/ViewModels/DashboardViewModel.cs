using System.Windows.Input;
using JobScheduleNotifications.Mobile.Services;
using JobScheduleNotifications.Mobile.Views;

namespace JobScheduleNotifications.Mobile.ViewModels;

public class DashboardViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;
    private int _totalCustomers;
    private int _totalJobs;
    private int _pendingJobs;
    private int _completedJobs;
    private string _businessName = string.Empty;
    private string _welcomeMessage = string.Empty;

    public DashboardViewModel(IAuthService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
        Title = "Dashboard";
        LoadDashboardDataCommand = new Command(async () => await LoadDashboardDataAsync());
        NavigateToCustomersCommand = new Command(async () => await NavigateToCustomersAsync());
        NavigateToScheduleJobCommand = new Command(async () => await NavigateToScheduleJobAsync());
        LogoutCommand = new Command(async () => await LogoutAsync());
    }

    public int TotalCustomers
    {
        get => _totalCustomers;
        set => SetProperty(ref _totalCustomers, value);
    }

    public int TotalJobs
    {
        get => _totalJobs;
        set => SetProperty(ref _totalJobs, value);
    }

    public int PendingJobs
    {
        get => _pendingJobs;
        set => SetProperty(ref _pendingJobs, value);
    }

    public int CompletedJobs
    {
        get => _completedJobs;
        set => SetProperty(ref _completedJobs, value);
    }

    public string BusinessName
    {
        get => _businessName;
        set => SetProperty(ref _businessName, value);
    }

    public string WelcomeMessage
    {
        get => _welcomeMessage;
        set => SetProperty(ref _welcomeMessage, value);
    }

    public ICommand LoadDashboardDataCommand { get; }
    public ICommand NavigateToCustomersCommand { get; }
    public ICommand NavigateToScheduleJobCommand { get; }
    public ICommand LogoutCommand { get; }

    private async Task LoadDashboardDataAsync()
    {
        if (IsBusy) return;

        try
        {
            SetBusy(true);
            // TODO: Load dashboard data from API
            // For now, using placeholder data
            TotalCustomers = 0;
            TotalJobs = 0;
            PendingJobs = 0;
            CompletedJobs = 0;
            BusinessName = "Your Business";
            WelcomeMessage = $"Welcome back, {BusinessName}!";
        }
        catch (Exception ex)
        {
            await _navigationService.ShowAlertAsync("Error", "Failed to load dashboard data");
            System.Diagnostics.Debug.WriteLine($"Dashboard Error: {ex.Message}");
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async Task NavigateToCustomersAsync()
    {
        await _navigationService.NavigateToAsync(nameof(CustomersPage));
    }

    private async Task NavigateToScheduleJobAsync()
    {
        await _navigationService.NavigateToAsync(nameof(ScheduleJobPage));
    }

    private async Task LogoutAsync()
    {
        if (IsBusy) return;

        try
        {
            SetBusy(true);
            await _authService.LogoutAsync();
            await _navigationService.NavigateToAsync(nameof(LoginPage));
        }
        catch (Exception ex)
        {
            await _navigationService.ShowAlertAsync("Error", "Failed to logout");
            System.Diagnostics.Debug.WriteLine($"Logout Error: {ex.Message}");
        }
        finally
        {
            SetBusy(false);
        }
    }
} 