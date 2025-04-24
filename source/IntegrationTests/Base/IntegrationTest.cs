using Api.Infrastructure.Data;
using Api.Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Server.Client;
using Server.Contracts.Client;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace IntegrationTests.Base;

public class IntegrationTest : IAsyncLifetime
{
    protected IServiceProvider ServiceProvider = null!;
    protected Server Server = null!;
    protected ILogger Logger = null!;
    protected IServerClient Client = null!;
    private IServiceScope _scope = null!;
    protected AppDbContext TestDb = null!;

    protected readonly CancellationToken CancellationToken = new CancellationTokenSource(TimeSpan.FromMinutes(5)).Token;

    public virtual async Task InitializeAsync()
    {
        Server = new Server();
        _scope = Server.Services.CreateScope();
        ServiceProvider = _scope.ServiceProvider;

        Logger = ServiceProvider.GetRequiredService<ILogger<IntegrationTest>>();
        TestDb = ServiceProvider.GetRequiredService<AppDbContext>();

        var client = Server.CreateClient();
        Client = new ServerClient(client);

        var db = ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync(CancellationToken);
        await ServiceProvider.SeedRolesAsync();
    }


    public virtual async Task DisposeAsync()
    {
        _scope.Dispose();
        await TestDb.DisposeAsync();
        await Server.DisposeAsync();
    }
}