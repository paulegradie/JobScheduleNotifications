using Api.Infrastructure.Auth.AccessPolicies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Infrastructure.Auth;

public static class RoleDefinitionExtensionMethods
{
    private static readonly string[] DefaultRoles;

    static RoleDefinitionExtensionMethods()
    {
        DefaultRoles = new[] { UserRoles.AdminRole, UserRoles.MemberRole };
    }

    public static async Task EnsureDefaultRoles(this WebApplication app)
    {
        await SetDefaultRoles(app.Services);
    }

    public static async Task EnsureDefaultRoles(this IServiceProvider serviceProvider)
    {
        await SetDefaultRoles(serviceProvider);
    }


    private static async Task SetDefaultRoles(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var role in DefaultRoles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}