using Api.Infrastructure.Auth;
using Api.Infrastructure.Auth.AccessPolicies;
using Api.Infrastructure.Data;
using Api.Infrastructure.DbTables;
using JobScheduleNotifications.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Composition;

public static class CompositionRoot
{
    public static void ComposeApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<IdentityOptions>(options => { options.User.RequireUniqueEmail = true; });

        // Register DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

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