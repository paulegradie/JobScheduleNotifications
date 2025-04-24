using Api.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Api.Infrastructure.EntityFramework;

public static class DbContextRegistration
{
    public static IServiceCollection AddConfiguredDbContextAndConventions(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
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

        services.RegisterEfConventions();
        return services;
    }
}