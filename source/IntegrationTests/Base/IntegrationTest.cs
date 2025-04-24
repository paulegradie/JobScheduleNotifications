using Api.Infrastructure.Auth;
using Api.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Server.Client;
using Server.Contracts.Client;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace IntegrationTests.Base;

public class IntegrationTest : IAsyncLifetime
{
    private IServiceScope scope = null!;
    private Server Server { get; set; }

    protected ILogger Logger { get; set; }

    protected IServerClient Client { get; private set; }

    protected CancellationToken CancellationToken { get; set; } = new CancellationTokenSource(TimeSpan.FromMinutes(5)).Token;

    public virtual async Task InitializeAsync()
    {
        Server = new Server();
        Logger = Server.Services.GetRequiredService<ILogger<IntegrationTest>>();
        TestDb = Server.Services.GetRequiredService<AppDbContext>();
        
        var client = Server.CreateClient();
        Client = new ServerClient(client);
        CancellationToken = new CancellationTokenSource(TimeSpan.FromMinutes(5)).Token;

        scope = Server.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync(CancellationToken);
        await Server.Services.EnsureDefaultRoles();
    }

    public AppDbContext TestDb { get; set; }


    public virtual async Task DisposeAsync()
    {
        scope?.Dispose();
        await TestDb.DisposeAsync();
        await Server.DisposeAsync();
    }
}