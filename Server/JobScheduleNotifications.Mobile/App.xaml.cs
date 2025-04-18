using JobScheduleNotifications.Mobile.Services;
using JobScheduleNotifications.Mobile.Views;

namespace JobScheduleNotifications.Mobile
{
    public partial class App : Application
    {
        private readonly IAuthService _authService;

        public App(IAuthService authService)
        {
            InitializeComponent();
            _authService = authService;

            // this async-void will run once the shell is live
            CheckAuthenticationState().Wait();
        }

        // ─── MAUI 8 windowing ──────────────────────────────────────────
        protected override Window CreateWindow(IActivationState? activationState)
        {
            // AppShell.xaml should define your <Shell …> with routes for
            // LoginPage, DashboardPage, etc.
            var shell = new AppShell();
            return new Window(shell) { Title = "Job Scheduler" };
        }
        // ───────────────────────────────────────────────────────────────

        private async Task CheckAuthenticationState()
        {
            var isAuthenticated = await _authService.IsAuthenticatedAsync();

            // now that Shell.Current is available, navigate to the right page
            if (isAuthenticated)
                await Shell.Current.GoToAsync(nameof(DashboardPage));
            else
                await Shell.Current.GoToAsync(nameof(LoginPage));
        }
    }
}