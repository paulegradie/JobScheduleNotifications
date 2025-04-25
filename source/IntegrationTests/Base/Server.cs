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
        builder.UseEnvironment("Test");
        builder.ConfigureTestServices(services =>
        {
            var uniqueId = Guid.NewGuid();

            // services.RemoveAll<AppDbContext>();
            // services.RemoveAll<DbContextOptions<AppDbContext>>();
            // services.RemoveAll<DbContextOptions>();

            services.AddDbContext<AppDbContext>((sp, options) => { options.UseInMemoryDatabase($"Data Source=IntegrationTesting{uniqueId}.db"); });
        });

        base.ConfigureWebHost(builder);
    }
}