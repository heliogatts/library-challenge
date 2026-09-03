using Microsoft.EntityFrameworkCore;
using LibraryApi.Data;

namespace LibraryApi.Shared.Extensions;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<LibraryDbContext>>();

        try
        {
            var pendingMigrations = await db.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Applying {Count} pending database migration(s)...", pendingMigrations.Count());
                await db.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully.");
            }
            else
            {
                logger.LogInformation("Database is up to date. No pending migrations.");
            }

            await SeedData.SeedAsync(db, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying database migrations or seeding data.");
            throw;
        }
    }
}
