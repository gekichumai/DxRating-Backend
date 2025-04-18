using DxRating.Database.Services;

namespace DxRating.Worker.Migrator;

public class Worker : BackgroundService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly DbMigrator _migrator;

    public Worker(IHostApplicationLifetime hostApplicationLifetime, DbMigrator migrator)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _migrator = migrator;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _migrator.EnsureDatabaseReadyAsync(stoppingToken);

        _hostApplicationLifetime.StopApplication();
    }
}
