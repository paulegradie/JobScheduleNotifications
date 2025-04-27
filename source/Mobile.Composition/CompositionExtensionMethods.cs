using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Mobile.Infrastructure.Persistence;
using Mobile.Infrastructure.Services;
using Mobile.UI.PageModels;
using Mobile.UI.Pages;
using Mobile.UI.RepositoryAbstractions;
using Server.Client;
using Server.Client.Base;
using Server.Contracts;

namespace Mobile.Composition;

public static class CompositionExtensionMethods
{
    public static void ComposeServices(this IServiceCollection services)
    {
#if DEBUG
        services.AddLogging(configure => configure.AddDebug());
#endif

        // Pages
        services.AddSingleton<HomePage>();
        services.AddSingleton<HomePageViewModel>();

        services.AddSingleton<DashboardPage>();
        services.AddSingleton<DashboardViewModel>();

        services.AddSingleton<CustomerPage>();
        services.AddSingleton<CustomerViewModel>();

        services.AddSingleton<CustomersPage>();
        services.AddSingleton<CustomersViewModel>();


        services.AddSingleton<ScheduledJobViewModel>();
        services.AddSingleton<ScheduledJobPage>();


        // Service
        services.AddDomainServices();

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