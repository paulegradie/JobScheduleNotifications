using Microsoft.Extensions.Logging;
using Mobile.Core.Repositories;
using Mobile.Core.Services;
using Mobile.Core.Utilities;
using Mobile.Infrastructure.Persistence;
using Mobile.UI.PageModels;
using Mobile.UI.Pages;
using Server.Contracts.Client;
using Server.Contracts.Client.Endpoints.Auth;

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

        services.AddTransient<INavigationUtility, NavigationUtility>();
        services.AddTransient<ICustomerRepository, CustomerRepository>();

        // HTTP + token storage + services
        services.AddTransient<AuthenticationHandler>();

        services.AddSingleton<ITokenStore, InMemoryTokenStore>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddHttpClient<IServerClient>().AddHttpMessageHandler<AuthenticationHandler>();
    }
}