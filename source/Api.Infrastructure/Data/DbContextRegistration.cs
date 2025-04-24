using Api.Infrastructure.DbTables.OrganizationModels;
using Api.Infrastructure.EntityFramework;
using Api.ValueTypes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Api.Infrastructure.Data;

public static class DbContextRegistration
{
    public static IServiceCollection AddConfiguredDbContext(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        if (!env.IsEnvironment("Test"))
        {
            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=DevDatabase.db";
                options.UseSqlite(connectionString);
                options.UseLazyLoadingProxies();
            });
        }

        services
            .AddIdentity<ApplicationUserRecord, IdentityRole<UserId>>(opts =>
            {
                // configure your password, lockout, etc. options here
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.RegisterEfConventions();

        return services;
    }
}