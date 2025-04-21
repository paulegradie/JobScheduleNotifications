using System.Security.Claims;
using System.Text;
using Api.Infrastructure.Auth.AccessPolicies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Api.Infrastructure.Auth;

public static class AuthenticationConfiguration
{
    public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthenticationOptions>(configuration.GetSection(AuthenticationOptions.Node));

        var authSection = configuration.GetSection(AuthenticationOptions.Node);
        var authOptions = authSection.Get<AuthenticationOptions>() ?? throw new Exception("Missing auth configuration");

        var key = Encoding.UTF8.GetBytes(authOptions.Key);

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = authOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = authOptions.Audience,
                    RoleClaimType = ClaimTypes.Role,
                    ClockSkew = TimeSpan.FromMinutes(2) // optional: prevents minor clock drift causing early expiry
                };
            });

        services.ConfigureRolePolicies();
    }

    private static void ConfigureRolePolicies(this IServiceCollection services)
    {
        services
            .AddAuthorizationBuilder()
            .AddPolicy(UserPolicies.AdminPolicy, policy =>
                policy.RequireClaim(ClaimTypes.Role, UserRoles.AdminRole));
    }
}