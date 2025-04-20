using Microsoft.Extensions.Logging;
using Mobile.Core.Interfaces;
using Mobile.Core.Services;
using Mobile.Infrastructure.Http;
using Mobile.Infrastructure.Persistence;
using Mobile.UI.PageModels;
using Mobile.UI.Pages;
using IAuthService = Mobile.Core.Services.IAuthService;
using ICustomerService = Mobile.Core.Services.ICustomerService;

namespace Mobile.Composition;

public static class CompositionExtensionMethods
{
    public static void ComposeServices(this IServiceCollection services)
    {
#if DEBUG
        services.AddLogging(configure => configure.AddDebug());
#endif

        services.AddSingleton<ModalErrorHandler>();

        services.AddSingleton<HomePage>();
        services.AddSingleton<HomePageViewModel>();

        services.AddSingleton<DashboardPage>();
        services.AddSingleton<DashboardViewModel>();

        services.AddSingleton<CustomerPage>();
        services.AddSingleton<CustomerViewModel>();

        services.AddSingleton<CustomersPage>();
        services.AddSingleton<CustomersViewModel>();

        services.AddTransient<INavigationService, NavigationService>();
        services.AddTransient<ICustomerService, CustomerService>();

        // HTTP + token storage + services
        services.AddSingleton<ITokenStore, InMemoryTokenStore>();
        services.AddScoped<IApiClient, ApiClient>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddHttpClient<ApiClient>();
    }
}