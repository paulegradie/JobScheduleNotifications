using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Mobile.Infrastructure;
using Mobile.Infrastructure.Persistence;
using Mobile.UI.Pages;
using Mobile.UI.Pages.Customers;
using Mobile.UI.Pages.Customers.ScheduledJobs;
using Mobile.UI.Pages.Customers.ScheduledJobs.JobOccurrences;
using Mobile.UI.RepositoryAbstractions;
using Server.Client;
using Server.Client.Base;
using Server.Contracts;

namespace Mobile.Composition;

public static class CompositionExtensionMethods
{
    private static void RegisterPageAndModel<TPage, TModel>(this IServiceCollection services) where TModel : class where TPage : class
    {
        services.AddSingleton<TPage>();
        services.AddSingleton<TModel>();
    }

    public static void ComposeServices(this IServiceCollection services)
    {
#if DEBUG
        services.AddLogging(configure => configure.AddDebug());
#endif

        services.RegisterPageAndModel<LandingPage, LandingPageModel>();
        services.RegisterPageAndModel<LoginPage, LoginViewModel>();
        services.RegisterPageAndModel<RegisterPage, RegisterViewModel>();
        services.RegisterPageAndModel<DashboardPage, DashboardViewModel>();

        services.RegisterPageAndModel<CustomerListPage, CustomerListModel>();
        services.RegisterPageAndModel<CustomerCreatePage, CustomerCreateModel>();
        services.RegisterPageAndModel<CustomerViewPage, CustomerViewModel>();
        services.RegisterPageAndModel<CustomerEditPage, CustomerEditModel>();

        services.RegisterPageAndModel<ScheduledJobEditPage, ScheduledJobEditModel>();
        services.RegisterPageAndModel<ScheduledJobCreatePage, ScheduledJobCreateModel>();
        services.RegisterPageAndModel<ScheduledJobListPage, ScheduledJobListModel>();
        services.RegisterPageAndModel<ScheduledJobViewPage, ScheduledJobViewModel>();

        services.RegisterPageAndModel<ViewJobOccurrencePage, ViewJobOccurrenceModel>();
        services.RegisterPageAndModel<InvoiceCreatePage, InvoiceCreateModel>();

        // Service
        services.AddRepositories();

        // Infra
        services.RegisterInfrastructure();

        // Clients
        services.AddTransient<AuthenticationHandler>();
        services.AddHttpClient<IServerClient, ServerClient>(ConfigureServerClient)
            .AddHttpMessageHandler<AuthenticationHandler>();
        services.AddHttpClient<IAuthClient, AuthClient>(ConfigureAuthClient);
    }

    private static ServerClient ConfigureServerClient(HttpClient client, IServiceProvider sp)
    {
        client.BaseAddress = new Uri("https://localhost:5001");

        // ← synchronously retrieve saved token
        var tokenRepo = sp.GetRequiredService<ITokenRepository>();
        var saved = tokenRepo.RetrieveTokenMeta().GetAwaiter().GetResult();

        if (saved == null) return new ServerClient(client);


        // preload if available from a previous session
        // this will make more sense once SetCurrentToken implements an out-of-process store
        // restore AuthEndpoint’s in-memory token
        if (sp.GetService<IAuthClient>() is AuthClient ac)
            ac.Auth.SetCurrentToken(saved);

        // prime the header
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", saved.AccessToken);

        return new ServerClient(client);
    }

    private static AuthClient ConfigureAuthClient(HttpClient client, IServiceProvider sp)
    {
        client.BaseAddress = new Uri("https://localhost:5001");
        return new AuthClient(client);
    }
}

public class ApiOptions
{
    public const string Node = "Api";
    public string ServerBaseUrl { get; set; } = default!;
    public string AuthBaseUrl { get; set; } = default!;
}