using Mobile.Infrastructure.Repositories;
using Mobile.UI.RepositoryAbstractions;
using Mobile.UI.Services;

namespace Mobile.Infrastructure.Services;

public static class ServiceRegistrationExtensionMethods
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddTransient<ICustomerService, CustomerService>();
        services.AddTransient<IJobService, JobService>();

        // Repositories
        services.AddTransient<INavigationRepository, NavigationRepository>();
        services.AddTransient<IJobRepository, JobRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        return services;
    }
}