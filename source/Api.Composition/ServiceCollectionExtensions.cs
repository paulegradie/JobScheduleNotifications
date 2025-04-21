using Api.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Composition;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ComposeServerDependencies(this IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        var targetType = typeof(IMapToExternalDto<,>);

        var types = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type is { IsAbstract: false, IsInterface: false })
            .Where(type => type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == targetType));

        foreach (var type in types)
        {
            var implementedInterfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == targetType);

            foreach (var iface in implementedInterfaces)
            {
                services.AddScoped(iface, type);
            }
        }

        return services;
    }
}