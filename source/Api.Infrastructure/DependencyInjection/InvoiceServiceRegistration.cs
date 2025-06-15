using Amazon.S3;
using Amazon.SimpleEmail;
using Api.Business.Repositories;
using Api.Business.Services;
using Api.Infrastructure.Configuration;
using Api.Infrastructure.Repositories;
using Api.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Infrastructure.DependencyInjection;

public static class InvoiceServiceRegistration
{
    public static IServiceCollection AddInvoiceServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure settings
        services.Configure<StorageConfiguration>(configuration.GetSection(StorageConfiguration.SectionName));
        services.Configure<EmailConfiguration>(configuration.GetSection(EmailConfiguration.SectionName));

        // Register repositories
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        // Register file storage service based on configuration
        var storageConfig = configuration.GetSection(StorageConfiguration.SectionName).Get<StorageConfiguration>() ?? new StorageConfiguration();
        
        if (storageConfig.Type == StorageType.S3)
        {
            // Configure AWS S3
            services.AddAWSService<IAmazonS3>();
            services.AddScoped<IFileStorageService, S3FileStorageService>();
        }
        else
        {
            services.AddScoped<IFileStorageService, LocalFileStorageService>();
        }

        // Register email service
        services.AddAWSService<IAmazonSimpleEmailService>();
        services.AddScoped<IEmailService, SESEmailService>();

        return services;
    }
}
