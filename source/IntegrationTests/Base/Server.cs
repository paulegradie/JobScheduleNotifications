using Api;
using Api.Infrastructure.Data;
using Api.Infrastructure.EntityFramework;
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

            services.AddDbContext<AppDbContext>((sp, options) => { options.UseSqlite($"Data Source=TestDatabase-{uniqueId}.db"); });

            services.AddScoped<AppDbContext>(sp =>
            {
                var options = sp.GetRequiredService<DbContextOptions<AppDbContext>>();
                var conventions = sp.GetRequiredService<IEnumerable<IEntityPropertyConvention>>();
                return new AppDbContext(conventions, options);
            });
        });

        base.ConfigureWebHost(builder);
    }
}