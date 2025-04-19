using Microsoft.Extensions.Logging;
using JobScheduleNotifications.Mobile.Services;
using JobScheduleNotifications.Mobile.ViewModels;
using JobScheduleNotifications.Mobile.Views;
using CommunityToolkit.Maui;

namespace JobScheduleNotifications.Mobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			 .UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		builder.Services.AddSingleton<AppShell>();

		// Register services
		builder.Services.AddSingleton<HttpClientService>();
		builder.Services.AddSingleton<IAuthService, AuthService>();
		builder.Services.AddSingleton<ICustomerService, CustomerService>();
		builder.Services.AddSingleton<IJobService, JobService>();
		builder.Services.AddSingleton<INavigationService, NavigationService>();

		// Register ViewModels
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<RegisterViewModel>();
		builder.Services.AddTransient<DashboardViewModel>();
		builder.Services.AddTransient<CustomerViewModel>();
		builder.Services.AddTransient<CustomersViewModel>();
		builder.Services.AddTransient<CustomerEditViewModel>();
		builder.Services.AddTransient<ScheduleJobViewModel>();

		// Register Pages
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<RegisterPage>();
		builder.Services.AddTransient<DashboardPage>();
		builder.Services.AddTransient<CustomerPage>();
		builder.Services.AddTransient<CustomersPage>();
		builder.Services.AddTransient<ScheduleJobPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
