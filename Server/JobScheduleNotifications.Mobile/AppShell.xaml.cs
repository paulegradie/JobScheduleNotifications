using JobScheduleNotifications.Mobile.Views;

namespace JobScheduleNotifications.Mobile;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		RegisterRoutes();
	}

	private void RegisterRoutes()
	{
		Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
		Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
		Routing.RegisterRoute(nameof(DashboardPage), typeof(DashboardPage));
		Routing.RegisterRoute(nameof(CustomerPage), typeof(CustomerPage));
		Routing.RegisterRoute(nameof(CustomersPage), typeof(CustomersPage));
		Routing.RegisterRoute(nameof(ScheduleJobPage), typeof(ScheduleJobPage));
	}
}
