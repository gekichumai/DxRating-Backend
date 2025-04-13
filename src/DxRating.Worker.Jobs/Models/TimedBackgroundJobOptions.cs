namespace DxRating.Worker.Jobs.Models;

public record TimedBackgroundJobOptions
{
    public TimeSpan Period { get; init; }

    public string JobName { get; init; } = string.Empty;

    public bool ExecuteOnStart { get; init; }
}
