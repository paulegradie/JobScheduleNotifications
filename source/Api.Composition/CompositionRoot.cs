using Api.Infrastructure.Auth;
using Api.Infrastructure.Auth.AccessPolicies;
using Api.Infrastructure.Data;
using Api.Infrastructure.DbTables;
using Api.Infrastructure.EntityFramework;
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
        services.RegisterEfConventions(); // Your extension to register IEntityPropertyConvention[]

        services.AddDbContext<AppDbContext>((sp, options) => { options.UseSqlite("Data Source=DevDatabase.db"); });

        services.AddScoped<AppDbContext>(sp =>
        {
            var options = sp.GetRequiredService<DbContextOptions<AppDbContext>>();
            var conventions = sp.GetRequiredService<IEnumerable<IEntityPropertyConvention>>();
            return new AppDbContext(conventions, options);
        });

        services
            .AddIdentity<ApplicationUserRecord, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.RegisterEfConventions();

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