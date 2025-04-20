using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Mobile.PageModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string _title = "Dashboard";

    [ObservableProperty]
    private int _totalCustomers;

    [ObservableProperty]
    private int _totalJobs;

    [ObservableProperty]
    private int _pendingJobs;

    [ObservableProperty]
    private int _completedJobs;

    [ObservableProperty]
    private string _businessName = string.Empty;

    [ObservableProperty]
    private string _welcomeMessage = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    public DashboardViewModel(IAuthService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
    }

    [RelayCommand]
    private async Task LoadDashboardData()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
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
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToCustomers()
    {
        await _navigationService.NavigateToAsync(nameof(CustomersPage));
    }

    [RelayCommand]
    private async Task NavigateToScheduleJob()
    {
        await _navigationService.NavigateToAsync(nameof(ScheduleJobPage));
    }

    [RelayCommand]
    private async Task Logout()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
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
            IsBusy = false;
        }
    }
}