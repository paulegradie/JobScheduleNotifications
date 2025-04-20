using JobScheduleNotifications.Core.Entities;
using JobScheduleNotifications.Core.Interfaces;
using JobScheduleNotifications.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JobScheduleNotifications.Composition;

public static class CompositionRoot
{
    public static void ComposeApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<JobScheduleDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        // Register Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IRepository<BusinessOwner>, Repository<BusinessOwner>>();
        services.AddScoped<IRepository<Customer>, Repository<Customer>>();
        services.AddScoped<IRepository<ScheduledJob>, Repository<ScheduledJob>>();
        services.AddScoped<IRepository<JobReminder>, Repository<JobReminder>>();

        // Register Application Services (to be added later)
        // services.AddScoped<IBusinessOwnerService, BusinessOwnerService>();
        // services.AddScoped<ICustomerService, CustomerService>();
        // services.AddScoped<IScheduledJobService, ScheduledJobService>();
    }
}