using System.Text.Json;
using System.Web;
using DxRating.Worker.Jobs.Models;
using DxRating.Worker.Jobs.Utils;

namespace DxRating.Worker.Jobs.Services;

public class DxDataUpdateAliasFetcher
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DxDataUpdateAliasFetcher> _logger;

    public DxDataUpdateAliasFetcher(HttpClient httpClient, ILogger<DxDataUpdateAliasFetcher> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Dictionary<string, List<string>>> GetAliases1Async()
    {
        var response = await _httpClient.GetAsync("https://raw.githubusercontent.com/lomotos10/GCM-bot/main/data/aliases/en/chuni.tsv");
        if (response.IsSuccessStatusCode is false)
        {
            throw new HttpRequestException("Failed to fetch aliases 1");
        }

        var tsv = await response.Content.ReadAsStringAsync();

        return tsv.Split('\n')
            .Select(line => line.Split('\t'))
            .ToDictionary(
                segments => segments[0],
                segments => segments.Skip(1).ToList());
    }

    public async Task<Dictionary<string, List<string>>> GetAliases2Async()
    {
        try
        {
            var response = await _httpClient.GetAsync("https://api.yuzuchan.moe/maimaidx/maimaidxalias");
            if (response.IsSuccessStatusCode is false)
            {
                throw new HttpRequestException("Failed to fetch aliases 2");
            }

            var jsonRaw = await response.Content.ReadAsStringAsync();
            var json = MaimaiDxAliasJsonSerializer.Deserialize(jsonRaw);
            if (json is null || json.StatusCode != 200)
            {
                throw new HttpRequestException("Failed to fetch aliases 2, the JSON is null or status code is not 200");
            }

            return json.Contents
                .ToDictionary(
                    x => x.Name,
                    x => x.Alias
                        .Select(HttpUtility.HtmlDecode)
                        .Where(a => a != null)
                        .Select(a => a!)
                        .ToList());
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Failed to fetch aliases 2, message: {Message}", e.Message);
        }

        return [];
    }

    public async Task<Dictionary<int, List<string>>> GetAliases3Async()
    {
        await using var stream = ResourceUtils.GetResourceAsync("aliases3.json");
        stream.Seek(0, SeekOrigin.Begin);

        var doc = await JsonDocument.ParseAsync(stream);
        var root = doc.RootElement;
        var objs = root.EnumerateObject();

        var aliases = new Dictionary<int, List<string>>();

        foreach (var obj in objs)
        {
            var id = int.Parse(obj.Name);
            var value = obj.Value
                .EnumerateArray()
                .Select(x => x.GetString()!)
                .ToList();

            aliases.Add(id, value);
        }

        return aliases;
    }

    public async Task<Dictionary<int, List<string>>> GetAliases4Async()
    {
        try
        {
            var response = await _httpClient.GetAsync("https://maimai.lxns.net/api/v0/maimai/alias/list");
            if (response.IsSuccessStatusCode is false)
            {
                throw new HttpRequestException("Failed to fetch aliases 4");
            }

            var jsonRaw = await response.Content.ReadAsStringAsync();
            var json = MaimaiLxnsNetAliasJsonSerializer.Deserialize(jsonRaw)
                       ?? throw new HttpRequestException("Failed to fetch aliases 4, the JSON is null");

            return json.Aliases
                .ToDictionary(
                    x => x.SongId,
                    x => x.Aliases);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Failed to fetch aliases 4, message: {Message}", e.Message);
        }

        return [];
    }

    public async Task<List<MaimaiOfficialSong>> GetMaimaiOfficialSongList()
    {
        var response = await _httpClient.GetAsync("https://maimai.sega.jp/data/maimai_songs.json");
        if (response.IsSuccessStatusCode is false)
        {
            throw new HttpRequestException("Failed to fetch maimai official songs");
        }

        var jsonRaw = await response.Content.ReadAsStringAsync();
        var officialSongs = MaimaiOfficialSongsSerializer.Deserialize(jsonRaw);

        return officialSongs;
    }
}
