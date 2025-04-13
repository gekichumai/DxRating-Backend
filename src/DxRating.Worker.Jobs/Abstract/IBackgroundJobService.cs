namespace DxRating.Worker.Jobs.Abstract;

public interface IBackgroundJobService
{
    public Task InvokeAsync(CancellationToken cancellationToken);
}
