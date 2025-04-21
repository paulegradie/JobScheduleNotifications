using System.Data.Common;
using Api;
using Api.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Base;

public class Server : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .ConfigureTestServices(services =>
            {
                var dbContextDescriptor =
                    services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (dbContextDescriptor is not null) services.Remove(dbContextDescriptor);

                var dbConnDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));
                if (dbConnDescriptor is not null) services.Remove(dbConnDescriptor);

                services.AddDbContext<AppDbContext>(opts =>
                    opts.UseSqlite("Data Source=:memory:")); // TODO: Replace with real db
            });

        base.ConfigureWebHost(builder);
    }
}