using System.Reflection;
using System.Text.Json.Serialization;
using DxRating.Common.Abstract;

// ReSharper disable InconsistentNaming

namespace DxRating.Common.Models.Data.Enums;

public record DxVersionType : ValueObject<string, DxVersionType>
{
    protected DxVersionType(string value) : base(value)
    {
    }

    [VersionTypeMap(0, "maimai")] public static readonly DxVersionType Maimai = new("maimai");
    [VersionTypeMap(1, "maimai-plus")] public static readonly DxVersionType MaimaiPlus = new("maimai PLUS");
    [VersionTypeMap(2, "green")] public static readonly DxVersionType GreeN = new("GreeN");
    [VersionTypeMap(3, "green-plus")] public static readonly DxVersionType GreeNPLUS = new("GreeN PLUS");
    [VersionTypeMap(4, "orange")] public static readonly DxVersionType Orange = new("ORANGE");
    [VersionTypeMap(5, "orange-plus")] public static readonly DxVersionType OrangePLUS = new("ORANGE PLUS");
    [VersionTypeMap(6, "pink")] public static readonly DxVersionType PiNK = new("PiNK");
    [VersionTypeMap(7, "pink-plus")] public static readonly DxVersionType PiNKPLUS = new("PiNK PLUS");
    [VersionTypeMap(8, "murasaki")] public static readonly DxVersionType MURASAKi = new("MURASAKi");
    [VersionTypeMap(9, "murasaki-plus")] public static readonly DxVersionType MURASAKiPLUS = new("MURASAKi PLUS");
    [VersionTypeMap(10, "milk")] public static readonly DxVersionType MiLK = new("MiLK");
    [VersionTypeMap(11, "milk-plus")] public static readonly DxVersionType MiLKPLUS = new("MiLK PLUS");
    [VersionTypeMap(12, "finale")] public static readonly DxVersionType FiNALE = new("FiNALE");
    [VersionTypeMap(13, "maimaidx")] public static readonly DxVersionType MaimaiDelux= new("maimaiでらっくす");
    [VersionTypeMap(14, "maimaidx-plus")] public static readonly DxVersionType MaimaiDeluxPlus = new("maimaiでらっくす PLUS");
    [VersionTypeMap(15, "splash")] public static readonly DxVersionType Splash = new("Splash");
    [VersionTypeMap(16, "splash-plus")] public static readonly DxVersionType SplashPLUS = new("Splash PLUS");
    [VersionTypeMap(17, "universe")] public static readonly DxVersionType UNiVERSE = new("UNiVERSE");
    [VersionTypeMap(18, "universe-plus")] public static readonly DxVersionType UNiVERSEPLUS = new("UNiVERSE PLUS");
    [VersionTypeMap(19, "festival")] public static readonly DxVersionType FESTiVAL = new("FESTiVAL");
    [VersionTypeMap(20, "festival-plus")] public static readonly DxVersionType FESTiVALPLUS = new("FESTiVAL PLUS");
    [VersionTypeMap(21, "buddies")] public static readonly DxVersionType BUDDiES = new("BUDDiES");
    [VersionTypeMap(22, "buddies-plus")] public static readonly DxVersionType BUDDiESPLUS = new("BUDDiES PLUS");
    [VersionTypeMap(23, "prism")] public static readonly DxVersionType PRiSM = new("PRiSM");
    [VersionTypeMap(24, "prism-plus")] public static readonly DxVersionType PRiSMPLUS = new("PRiSM PLUS");

    private static Dictionary<DxVersionType, VersionTypeMapAttribute> VersionAttributeMap { get; } = typeof(DxVersionType)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .ToDictionary(
            x => (DxVersionType)x.GetValue(null)!,
            x => x.GetCustomAttribute<VersionTypeMapAttribute>()!);

    public static Dictionary<int, DxVersionType> VersionIdMap { get; } =
        VersionAttributeMap.ToDictionary(
            x => x.Value.Id,
            x => x.Key);

    public static Dictionary<string, DxVersionType> VersionSlugMap { get; } =
        VersionAttributeMap.ToDictionary(
            x => x.Value.Slug,
            x => x.Key);

    [JsonIgnore]
    public int Id => VersionAttributeMap[this].Id;
    [JsonIgnore]
    public string Slug => VersionAttributeMap[this].Slug;

    public static DxVersionType FromId(int id)
    {
        VersionIdMap.TryGetValue(id, out var version);
        return version ?? throw new ArgumentOutOfRangeException(nameof(id), id, "Invalid version ID");
    }

    public static DxVersionType FromSlug(string slug)
    {
        VersionSlugMap.TryGetValue(slug, out var version);
        return version ?? throw new ArgumentOutOfRangeException(nameof(slug), slug, "Invalid version slug");
    }
}

public class DxVersionTypeJsonConverter : ValueObjectJsonConverter<string, DxVersionType>;

[AttributeUsage(AttributeTargets.Field)]
internal sealed class VersionTypeMapAttribute(int id, string slug) : Attribute
{
    public int Id { get; } = id;
    public string Slug { get; } = slug;
}
