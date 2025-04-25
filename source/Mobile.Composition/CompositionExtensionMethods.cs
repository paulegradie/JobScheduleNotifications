using Microsoft.Extensions.Logging;
using Mobile.Core.Repositories;
using Mobile.Core.Services;
using Mobile.Core.Utilities;
using Mobile.Infrastructure.Persistence;
using Mobile.UI.PageModels;
using Mobile.UI.Pages;
using Mobile.UI.RepositoryAbstractions;
using Server.Client.Base;
using Server.Contracts.Client;

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

        services.AddTransient<INavigationRepository, NavigationRepository>();

        services.AddTransient<ICustomerRepository, CustomerRepository>();
        services.AddSingleton<ITokenRepository, InMemoryTokenRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddTransient<AuthenticationHandler>();
        services.AddHttpClient<IServerClient>().AddHttpMessageHandler<AuthenticationHandler>();
    }
}