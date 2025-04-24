using Microsoft.Extensions.DependencyInjection;

namespace Api.Composition;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ComposeServerDependencies(this IServiceCollection services)
    {
        return services;
    }
}