using System.Reflection;
using Api.Business.Interfaces;
using Api.Infrastructure.Data;
using Api.Infrastructure.DbTables;
using Api.Infrastructure.DbTables.OrganizationModels;
using Api.Infrastructure.Repositories;
using Api.Infrastructure.Services;
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

    public static void AddBusinessServices(IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();
    }

    public static void AddCrudRepositories(IServiceCollection services)
    {
        services.AddScoped(typeof(ICrudRepository<>), typeof(CrudRepository<>));
        services.AddScoped<ICrudRepository<Customer>, CrudRepository<Customer>>();
        services.AddScoped<ICrudRepository<ScheduledJob>, CrudRepository<ScheduledJob>>();
        services.AddScoped<ICrudRepository<JobReminder>, CrudRepository<JobReminder>>();
    }

    public static void AddMappers(IServiceCollection services)
    {
        services.AddScoped<IMapperFactory, MapperFactory>();

        var entry = Assembly.GetExecutingAssembly()!;
        var toScan = new List<Assembly> { entry };
        toScan.AddRange(entry
            .GetReferencedAssemblies()             // all assemblies your EXE (or WebHost) references
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