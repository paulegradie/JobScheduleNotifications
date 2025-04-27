using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts;
using Server.Contracts.Endpoints.Auth.Contracts;

namespace Mobile.UI.PageModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IServerClient _serverClient;
    private readonly INavigationRepository _navigationUtility;
    private readonly ITokenRepository _tokenRepository;

    [ObservableProperty] private string _title = "Dashboard";

    [ObservableProperty] private int _totalCustomers;

    [ObservableProperty] private int _totalJobs;

    [ObservableProperty] private int _pendingJobs;

    [ObservableProperty] private int _completedJobs;

    [ObservableProperty] private string _businessName = string.Empty;

    [ObservableProperty] private string _welcomeMessage = string.Empty;

    [ObservableProperty] private bool _isBusy;

    public DashboardViewModel(IServerClient serverClient, INavigationRepository navigationUtility, ITokenRepository tokenRepository)
    {
        _serverClient = serverClient;
        _navigationUtility = navigationUtility;
        _tokenRepository = tokenRepository;
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
            var token = await _tokenRepository.RetrieveTokenMeta();
            if (token == null) throw new Exception("No token found");
            await _serverClient.Auth.LogoutAsync(new SignOutRequest(token.Email), CancellationToken.None);

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