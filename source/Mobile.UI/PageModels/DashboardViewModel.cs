using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.Core.Services;
using Mobile.Core.Utilities;
using Mobile.UI.Pages;
using Server.Contracts.Client;
using Server.Contracts.Client.Endpoints.Auth;

namespace Mobile.UI.PageModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IServerClient _serverClient;
    private readonly INavigationUtility _navigationUtility;

    [ObservableProperty] private string _title = "Dashboard";

    [ObservableProperty] private int _totalCustomers;

    [ObservableProperty] private int _totalJobs;

    [ObservableProperty] private int _pendingJobs;

    [ObservableProperty] private int _completedJobs;

    [ObservableProperty] private string _businessName = string.Empty;

    [ObservableProperty] private string _welcomeMessage = string.Empty;

    [ObservableProperty] private bool _isBusy;

    public DashboardViewModel(IServerClient serverClient, INavigationUtility navigationUtility)
    {
        _serverClient = serverClient;
        _navigationUtility = navigationUtility;
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
            await _navigationUtility.ShowAlertAsync("Error", "Failed to load dashboard data");
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
        await _navigationUtility.NavigateToAsync(nameof(CustomersPage));
    }

    [RelayCommand]
    private async Task NavigateToScheduleJob()
    {
        await _navigationUtility.NavigateToAsync(nameof(ScheduleJobPage));
    }

    [RelayCommand]
    private async Task Logout()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var currentUser = await _serverClient.Auth.GetCurrentUserEmailAsync();
            await _serverClient.Auth.LogoutAsync(new SignOutRequest(currentUser.Email));
            await _navigationUtility.NavigateToAsync(nameof(LoginPage));
        }
        catch (Exception ex)
        {
            await _navigationUtility.ShowAlertAsync("Error", "Failed to logout");
            System.Diagnostics.Debug.WriteLine($"Logout Error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}