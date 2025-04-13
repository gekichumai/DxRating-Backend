using DxRating.Worker.Jobs.Abstract;
using DxRating.Worker.Jobs.Jobs;
using DxRating.Worker.Jobs.Services;

namespace DxRating.Worker.Jobs;

public static class Configurator
{
    public static void AddBackgroundJobs(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpClient<DxDataUpdateAliasFetcher>(client =>
        {
            client.DefaultRequestHeaders.Add(
                "User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36 Edg/135.0.0.0");
        });

        builder.Services.AddHostedService<DxDataUpdateJob>();
        builder.Services.AddKeyedScoped<IBackgroundJobService, DxDataUpdateJobService>(nameof(DxDataUpdateJob));
    }
}
