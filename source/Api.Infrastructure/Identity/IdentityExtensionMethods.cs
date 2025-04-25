using Api.Infrastructure.Data;
using Api.Infrastructure.DbTables.OrganizationModels;
using Api.ValueTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Infrastructure.Identity;

/// <summary>
/// Extension methods for setting up ASP.NET Core Identity and seeding roles.
/// </summary>
public static class IdentityExtensions
{
    /// <summary>
    /// Registers ASP.NET Core Identity with ApplicationUserRecord and IdentityRole<IdentityUserId>,
    /// and configures password, lockout, and user options.
    /// </summary>
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUserRecord, IdentityRole<IdentityUserId>>(options =>
            {
                // configure your password, lockout, etc.
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    /// <summary>
    /// Ensures all roles defined in Roles.All are present in the database.
    /// Generates a new IdentityUserId for each role to avoid duplicate key conflicts.
    /// </summary>
    public static async Task SeedRolesAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole<IdentityUserId>>>();

        foreach (var roleName in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new IdentityRole<IdentityUserId>(roleName)
                {
                    Id = IdentityUserId.New()
                };

                await roleManager.CreateAsync(role);
            }
        }
    }
}