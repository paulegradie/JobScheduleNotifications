using Api.Infrastructure.Auth;
using Api.Infrastructure.Auth.AccessPolicies;
using Api.Infrastructure.Data;
using Api.Infrastructure.DbTables;
using JobScheduleNotifications.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Composition;

public static class CompositionRoot
{
    public static void ComposeApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Register Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IRepository<BusinessOwner>, Repository<BusinessOwner>>();
        services.AddScoped<IRepository<Customer>, Repository<Customer>>();
        services.AddScoped<IRepository<ScheduledJob>, Repository<ScheduledJob>>();
        services.AddScoped<IRepository<JobReminder>, Repository<JobReminder>>();
        services.AddSingleton<IJwt, Jwt>();
        services.AddTransient<IAuthenticator, Authenticator>();
    }
}