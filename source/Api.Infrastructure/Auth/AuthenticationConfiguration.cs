using System.Security.Claims;
using System.Text;
using Api.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Api.Infrastructure.Auth
{
    public static class AuthenticationConfiguration
    {
        /// <summary>
        /// Configures JWT-based authentication and authorization.
        /// </summary>
        public static IServiceCollection ConfigureAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // bind options
            services.Configure<AuthenticationOptions>(
                configuration.GetSection(AuthenticationOptions.Node));

            services.AddSingleton<IJwt, Jwt>();
            services.AddTransient<IAuthenticator, Authenticator>();

            // read settings
            var authOptions = configuration
                                  .GetSection(AuthenticationOptions.Node)
                                  .Get<AuthenticationOptions>()
                              ?? throw new InvalidOperationException("Missing authentication configuration");

            var keyBytes = Encoding.UTF8.GetBytes(authOptions.Key);

            // configure JWT bearer
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
                        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

                        ValidateIssuer = true,
                        ValidIssuer = authOptions.Issuer,

                        ValidateAudience = true,
                        ValidAudience = authOptions.Audience,

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(2),

                        RoleClaimType = ClaimTypes.Role,
                        NameClaimType = ClaimTypes.NameIdentifier
                    };
                });

            // register policies
            services.AddRolePolicies();

            return services;
        }
    }
}