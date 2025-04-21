using Api.Infrastructure.DbTables;
using Api.Infrastructure.EntityFramework;
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
            });
        }

        services
            .AddIdentity<ApplicationUserRecord, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.RegisterEfConventions();

        return services;
    }
}