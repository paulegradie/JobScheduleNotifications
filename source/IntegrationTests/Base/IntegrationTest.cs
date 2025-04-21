using Api.Infrastructure.Auth;
using Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Server.Client;
using Server.Contracts.Client;

namespace IntegrationTests.Base;

public class IntegrationTest : IAsyncLifetime
{
    private IDbContextTransaction transaction = null!;

    private IServiceScope scope = null!;
    private Server Server { get; set; }

    protected ILogger Logger { get; set; }

    protected IServerClient Client { get; private set; }

    protected CancellationToken CancellationToken { get; set; } =
        new CancellationTokenSource(TimeSpan.FromMinutes(5)).Token;

    public virtual async Task InitializeAsync()
    {
        Server = new Server();
        Logger = Server.Services.GetRequiredService<ILogger<IntegrationTest>>();

        var client = Server.CreateClient();
        Client = new ServerClient(client);
        CancellationToken = new CancellationTokenSource(TimeSpan.FromMinutes(5)).Token;

        // Create a service scope to resolve scoped services
        scope = Server.Services.CreateScope();

        // Get the DbContext from the scope
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync(CancellationToken);

        // Use the scoped provider for this operation
        await scope.ServiceProvider.EnsureDefaultRoles();

        transaction = await db.Database.BeginTransactionAsync(CancellationToken);
    }


    public virtual async Task DisposeAsync()
    {
        if (transaction is not null)
        {
            await transaction.RollbackAsync(CancellationToken);
            await transaction.DisposeAsync();
        }

        scope?.Dispose();

        // Uncomment if you need to dispose the server
        // await Server.DisposeAsync();
    }
}