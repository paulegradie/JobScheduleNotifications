using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Infrastructure.EntityFramework;

public static class EntityConventionRegistrationExtensionMethods
{
    public static IServiceCollection RegisterEfConventions(this IServiceCollection services)
    {
        services.AddSingleton<IEntityConventionApplier, EntityConventionApplier>();

        var conventionTypes = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false } && typeof(IEntityPropertyConvention).IsAssignableFrom(t));

        foreach (var type in conventionTypes)
        {
            services.AddSingleton(typeof(IEntityPropertyConvention), type);
        }

        return services;
    }
}