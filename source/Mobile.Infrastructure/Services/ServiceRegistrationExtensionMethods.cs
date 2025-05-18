using Mobile.Infrastructure.Repositories;
using Mobile.UI.RepositoryAbstractions;

namespace Mobile.Infrastructure.Services;

public static class ServiceRegistrationExtensionMethods
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<INavigationRepository, NavigationRepository>();
        services.AddTransient<IJobRepository, JobRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IJobOccurrenceRepository, JobOccurrenceRepository>();
        return services;
    }
}