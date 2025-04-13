using DxRating.Common.Models.Data;
using DxRating.Common.Models.Data.Enums;
using DxRating.Worker.Jobs.Abstract;
using DxRating.Worker.Jobs.Models;
using DxRating.Worker.Jobs.Services;
using DxRating.Worker.Jobs.Utils;

namespace DxRating.Worker.Jobs.Jobs;

public class DxDataUpdateJob(IServiceProvider serviceProvider) : TimedBackgroundJob(
    serviceProvider,
    new TimedBackgroundJobOptions
    {
        Period = TimeSpan.FromHours(1),
        ExecuteOnStart = true,
        JobName = nameof(DxDataUpdateJob)
    })
{
}

public class DxDataUpdateJobService : IBackgroundJobService
{
    private readonly DxDataUpdateAliasFetcher _dxDataUpdateAliasFetcher;
    private readonly ILogger<DxDataUpdateJobService> _logger;

    public DxDataUpdateJobService(
        DxDataUpdateAliasFetcher dxDataUpdateAliasFetcher,
        ILogger<DxDataUpdateJobService> logger)
    {
        _dxDataUpdateAliasFetcher = dxDataUpdateAliasFetcher;
        _logger = logger;
    }

    public async Task InvokeAsync(CancellationToken cancellationToken)
    {
        var aliases1 = await _dxDataUpdateAliasFetcher.GetAliases1Async();
        var aliases2 = await _dxDataUpdateAliasFetcher.GetAliases2Async();
        var aliases3 = await _dxDataUpdateAliasFetcher.GetAliases3Async();
        var aliases4 = await _dxDataUpdateAliasFetcher.GetAliases4Async();

        var nameAliasMap = DictionaryUtils.Merge(aliases1, aliases2);
        var idAliasMap = DictionaryUtils.Merge(aliases3, aliases4);

        // TODO: Fetch multiver internal level values...
        // TODO: Fetch overridden sheet-specific release dates...

        var maimaiOfficialSongs = await _dxDataUpdateAliasFetcher.GetMaimaiOfficialSongList();

        // TODO: Read original data
        var dxData = new DxData();
    }
}
