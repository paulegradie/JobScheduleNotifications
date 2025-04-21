using Api;
using Api.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IntegrationTests.Base;

public class Server : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureTestServices(services =>
        {
            var uniqueId = Guid.NewGuid(); // or pass this in

            services.RemoveAll<AppDbContext>();
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<DbContextOptions>();

            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase($"Data Source=IntegrationTesting{uniqueId}.db");

                // services.AddDbContext<AppDbContext>((sp, options) => { options.UseSqlite($"Database-{uniqueId}"); });
                // services.AddDbContext<AppDbContext>(options =>
                //     options.UseInMemoryDatabase("TestDb"));
            });
        });

        base.ConfigureWebHost(builder);
    }
}