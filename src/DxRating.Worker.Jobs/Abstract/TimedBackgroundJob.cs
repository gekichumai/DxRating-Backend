using System.Diagnostics;
using DxRating.Worker.Jobs.Models;

namespace DxRating.Worker.Jobs.Abstract;

public abstract class TimedBackgroundJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    private readonly PeriodicTimer _timer;
    private readonly bool _executeOnStart;
    private readonly string _jobName;

    private readonly ILogger<TimedBackgroundJob> _logger;

    protected TimedBackgroundJob(IServiceProvider serviceProvider, TimedBackgroundJobOptions options)
    {
        _serviceProvider = serviceProvider;

        _timer = new PeriodicTimer(options.Period);
        _executeOnStart = options.ExecuteOnStart;
        _jobName = options.JobName;

        var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
        _logger = loggerFactory.CreateLogger<TimedBackgroundJob>();

        _logger.LogInformation("Background Job {JobName} initialized", _jobName);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            if (_executeOnStart)
            {
                await ExecuteBackgroundJobScopeWrapperAsync(stoppingToken);
            }

            while (await _timer.WaitForNextTickAsync(stoppingToken))
            {
                await ExecuteBackgroundJobScopeWrapperAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Background Job {JobName} stopped", _jobName);
        }
    }

    private async Task ExecuteBackgroundJobScopeWrapperAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Invoke background Job {JobName}", _jobName);
            var sw = Stopwatch.StartNew();

            await using var scope = _serviceProvider.CreateAsyncScope();
            var jobService = scope.ServiceProvider.GetRequiredKeyedService<IBackgroundJobService>(_jobName);
            await jobService.InvokeAsync(cancellationToken);

            sw.Stop();
            _logger.LogInformation("Background Job {JobName} completed in {ElapsedMilliseconds} ms", _jobName, sw.ElapsedMilliseconds);
        }
        catch (OperationCanceledException e)
        {
            _logger.LogWarning("An invocation of background Job {JobName} was canceled, message: {CancellationMessage}", _jobName, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while executing background Job {JobName}, message: {ErrorMessage}", _jobName, e.Message);
        }
    }
}
