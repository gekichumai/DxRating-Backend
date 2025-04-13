using System.Text.Json.Serialization;
using DxRating.Common.Models.Data.Enums;

namespace DxRating.Common.Models.Data;

public record DxSong
{
    [JsonPropertyName("songId")]
    public string SongId { get; set; } = null!;

    [JsonPropertyName("category")]
    [JsonConverter(typeof(DxCategoryTypeJsonConverter))]
    public DxCategoryType Category { get; set; } = null!;

    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [JsonPropertyName("artist")]
    public string Artist { get; set; } = null!;

    [JsonPropertyName("bpm")]
    public int? Bpm { get; set; }

    [JsonPropertyName("imageName")]
    public string ImageName { get; set; } = null!;

    [JsonPropertyName("isNew")]
    public bool IsNew { get; set; }

    [JsonPropertyName("isLocked")]
    public bool IsLocked { get; set; }

    [JsonPropertyName("searchAcronyms")]
    public List<string> SearchAcronyms { get; set; } = [];

    [JsonPropertyName("sheets")]
    public List<Sheet> Sheets { get; set; } = [];
}

public record Sheet
{
    [JsonPropertyName("internalId")]
    public int? InternalId { get; set; }

    [JsonPropertyName("type")]
    [JsonConverter(typeof(DxSheetTypeJsonConverter))]
    public DxSheetType Type { get; set; } = null!;

    [JsonPropertyName("releaseDate")]
    public DateTimeOffset? ReleaseDate { get; set; }

    [JsonPropertyName("difficulty")]
    [JsonConverter(typeof(DxDifficultyTypeJsonConverter))]
    public DxDifficultyType DifficultyType { get; set; } = null!;

    [JsonPropertyName("level")]
    public string Level { get; set; } = null!;

    [JsonPropertyName("internalLevelValue")]
    public int InternalLevelValue { get; set; }

    [JsonPropertyName("noteDesigner")]
    public string? NoteDesigner { get; set; }

    [JsonPropertyName("noteCounts")]
    public NoteCounts NoteCounts { get; set; } = null!;

    [JsonPropertyName("regions")]
    public Regions Regions { get; set; } = null!;

    [JsonPropertyName("isSpecial")]
    public bool IsSpecial { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; } = null!;

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}

public record NoteCounts
{
    [JsonPropertyName("tap")]
    public int? Tap { get; set; }

    [JsonPropertyName("hold")]
    public int? Hold { get; set; }

    [JsonPropertyName("slide")]
    public int? Slide { get; set; }

    [JsonPropertyName("touch")]
    public int? Touch { get; set; }

    [JsonPropertyName("break")]
    public int? Break { get; set; }

    [JsonPropertyName("total")]
    public int? Total { get; set; }
}

public record Regions
{
    [JsonPropertyName("jp")]
    public bool Japan { get; set; }

    [JsonPropertyName("intl")]
    public bool International { get; set; }

    [JsonPropertyName("cn")]
    public bool China { get; set; }
}
