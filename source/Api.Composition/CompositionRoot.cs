using System.Reflection;
using Api.Business.Repositories;
using Api.Business.Repositories.Internal;
using Api.Business.Services;
using Api.Infrastructure.Data;
using Api.Infrastructure.DbTables.Jobs;
using Api.Infrastructure.DbTables.OrganizationModels;
using Api.Infrastructure.Repositories;
using Api.Infrastructure.Repositories.Mapping;
using Api.Infrastructure.Services;
using Api.ValueTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Composition;

public static class CompositionRoot
{
    public static void ComposeApplication(this IServiceCollection services, IConfiguration configuration)
    {
        AddCrudRepositories(services);
        AddMappers(services);
        AddBusinessServices(services);
    }

    private static void AddBusinessServices(IServiceCollection services)
    {
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IJobSchedulingService, JobSchedulingService>();
        services.AddScoped<IRecurrenceCalculator, SimpleRecurrenceCalculator>();
    }

    private static void AddCrudRepositories(IServiceCollection services)
    {
        services
            .AddScoped(typeof(ICrudRepository<,>), typeof(CrudRepository<,>))
            .AddScoped<ICrudRepository<Customer, CustomerId>, CrudRepository<Customer, CustomerId>>()
            .AddScoped<ICrudRepository<CustomerUser, CustomerId>, CrudRepository<CustomerUser, CustomerId>>()
            .AddScoped<ICrudRepository<CustomerUser, IdentityUserId>, CrudRepository<CustomerUser, IdentityUserId>>()
            .AddScoped<ICrudRepository<JobReminder, JobReminderId>, CrudRepository<JobReminder, JobReminderId>>()
            .AddScoped<IScheduledJobDefinitionRepository, ScheduledJobDefinitionRepository>()
            .AddScoped<IJobOccurrenceRepository, JobOccurrenceRepository>()
            .AddScoped<IJobReminderRepository, JobReminderRepository>();
    }

    private static void AddMappers(IServiceCollection services)
    {
        services.AddScoped<IMapperFactory, MapperFactory>();

        var entry = Assembly.GetExecutingAssembly()!;
        var toScan = new List<Assembly> { entry };
        toScan.AddRange(entry
            .GetReferencedAssemblies() // all assemblies your EXE (or WebHost) references
            .Where(a => a.Name!.StartsWith("Api")) // filter to your “Api.*” projects
            .Select(Assembly.Load));
        var mapFrom = typeof(IMapFrom<,>);
        var mappings = toScan
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces(), (t, i) => new { Type = t, Iface = i })
            .Where(x => x.Iface.IsGenericType
                        && x.Iface.GetGenericTypeDefinition() == mapFrom);

        // 3) Register each:
        foreach (var m in mappings)
            services.AddScoped(m.Iface, m.Type);
    }
}