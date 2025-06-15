﻿﻿﻿using Mobile.Infrastructure.Repositories;
using Mobile.UI.RepositoryAbstractions;
using Mobile.UI.Navigation;

namespace Mobile.Infrastructure;

public static class RegistrationExtensionMethods
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<INavigationRepository, NavigationRepository>();
        services.AddTransient<IAlertRepository, AlertRepository>();
        services.AddTransient<ITypeSafeNavigationRepository, TypeSafeNavigationRepository>();
        services.AddTransient<IJobRepository, JobRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IJobOccurrenceRepository, JobOccurrenceRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IJobCompletedPhotoRepository, JobCompletedPhotoRepository>();
        return services;
    }
}