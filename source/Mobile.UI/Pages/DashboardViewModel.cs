using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages.Base;
using Mobile.UI.Pages.Customers;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts;
using Server.Contracts.Endpoints.Auth.Contracts;
using Server.Contracts.Endpoints.JobPhotos.Contracts;

namespace Mobile.UI.Pages;

public partial class DashboardViewModel : BaseViewModel
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

    public DashboardViewModel(
        IServerClient serverClient,
        INavigationRepository navigationUtility,
        ITokenRepository tokenRepository)
    {
        _serverClient = serverClient;
        _navigationUtility = navigationUtility;
        _tokenRepository = tokenRepository;
    }

    [RelayCommand]
    private async Task LoadDashboardData()
    {
        await RunWithSpinner(async () =>
        {
            var dashboardResponse = await _serverClient
                .DashboardEndpoint
                .GetDashboardContent(new GetDashboardContentRequest(), CancellationToken.None);

            if (dashboardResponse.IsSuccess)
            {
                var content = dashboardResponse.Value.DashboardDto!;
                TotalCustomers = content.Customers.Count;
                TotalJobs = content.TotalJobsAcrossCustomers;
                PendingJobs = content.PendingJobs;
                BusinessName = content.BusinessName;
                WelcomeMessage = $"Welcome {content.CurrentUser}";
                CompletedJobs = content.TotalCompletedJobs;
                return;
            }

            await _navigationUtility.ShowAlertAsync("Error", "Failed to load dashboard data");
            System.Diagnostics.Debug.WriteLine($"Dashboard Error");
        });
    }

    [RelayCommand]
    private async Task NavigateToCustomers()
    {
        await _navigationUtility.GoToAsync(nameof(CustomerListPage));
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

            await _navigationUtility.GoToAsync(nameof(LoginPage));
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