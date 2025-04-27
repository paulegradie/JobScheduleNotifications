using Mobile.UI.RepositoryAbstractions;

namespace Mobile.Infrastructure.Persistence;

public static class InfraRegistrationExtensionMethods
{
    public static IServiceCollection RegisterInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ITokenRepository, InMemoryTokenRepository>();
        return services;
    }
}