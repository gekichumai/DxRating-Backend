using DxRating.Common.Abstract;

// ReSharper disable InconsistentNaming

namespace DxRating.Common.Models.Data.Enums;

public record DxCategoryType : ValueObject<string, DxCategoryType>
{
    protected DxCategoryType(string value) : base(value)
    {
    }

    public static readonly DxCategoryType PopsAndAnime = new("POPS＆アニメ");
    public static readonly DxCategoryType NiconicoAndVocaloid = new("niconico＆ボーカロイド");
    public static readonly DxCategoryType TouhouProject = new("東方Project");
    public static readonly DxCategoryType GameAndVariety = new("ゲーム＆バラエティ");
    public static readonly DxCategoryType Maimai = new("maimai");
    public static readonly DxCategoryType OngekiAndCHUNITHM = new("オンゲキ＆CHUNITHM");
    public static readonly DxCategoryType Enkaijou = new("宴会場");
}

public class DxCategoryTypeJsonConverter : ValueObjectJsonConverter<string, DxCategoryType>;
