using Microsoft.Extensions.DependencyInjection;

namespace Api.Infrastructure.Data;

public static class DatabaseMigrator
{
    public static async Task EnsureAndMigrateDatabase(
        this IServiceProvider provider,
        bool preDelete = false,
        CancellationToken cancellationToken = default)
    {
        using var scope = provider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (preDelete) await dbContext.Database.EnsureDeletedAsync(cancellationToken);
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
    }
}