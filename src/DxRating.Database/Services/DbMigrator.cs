using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DxRating.Database.Services;

public class DbMigrator
{
    private readonly ILogger<DbMigrator> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostEnvironment _hostEnvironment;

    private const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    public DbMigrator(
        ILogger<DbMigrator> logger,
        IServiceProvider serviceProvider,
        IHostEnvironment hostEnvironment)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _hostEnvironment = hostEnvironment;
    }

    public async Task EnsureDatabaseReadyAsync(CancellationToken stoppingToken)
    {
        // ReSharper disable once ExplicitCallerInfoArgument
        using var activity = ActivitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DxDbContext>();

            await EnsureDatabaseAsync(dbContext, stoppingToken);
            await RunMigrationAsync(dbContext, stoppingToken);

            if (_hostEnvironment.IsDevelopment())
            {
                await SeedDevelopmentDataAsync(dbContext, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }
    }

    private async Task EnsureDatabaseAsync(DxDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        var result = await strategy.ExecuteAsync(true,
            async (context, state, ct) =>
            {
                var dbCreator = context.GetService<IRelationalDatabaseCreator>();

                if (await dbCreator.ExistsAsync(ct) is false)
                {
                    _logger.LogInformation("Creating database");
                    await dbCreator.CreateAsync(ct);
                }

                return state;
            },
            async (context, _, ct) =>
            {
                var dbCreator = context.GetService<IRelationalDatabaseCreator>();
                var exists = await dbCreator.ExistsAsync(ct);

                return new ExecutionResult<bool>(exists, exists);
            }, cancellationToken);

        if (result is false)
        {
            _logger.LogCritical("Database creation failed");
            throw new InvalidOperationException("Failed to ensure database is created.");
        }
    }

    private async Task RunMigrationAsync(DxDbContext dbContext, CancellationToken cancellationToken)
    {
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
        if (pendingMigrations.Any() is false)
        {
            return;
        }

        try
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Failed to migrate database. {ErrorMessage}", e.Message);
        }
    }

    private static async Task SeedDevelopmentDataAsync(DxDbContext dbContext, CancellationToken cancellationToken)
    {
        _ = dbContext;
        _ = cancellationToken;
        await Task.CompletedTask;
    }
}
