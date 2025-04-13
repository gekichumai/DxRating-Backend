using DxRating.Common.Abstract;

namespace DxRating.Common.Models.Data.Enums;

public record DxDifficultyType : ValueObject<string, DxDifficultyType>
{
    protected DxDifficultyType(string value) : base(value)
    {
    }

    public static readonly DxDifficultyType Basic = new("basic");
    public static readonly DxDifficultyType Advanced = new("advanced");
    public static readonly DxDifficultyType Expert = new("expert");
    public static readonly DxDifficultyType Master = new("master");
    public static readonly DxDifficultyType ReMaster = new("remaster");
}

public class DxDifficultyTypeJsonConverter : ValueObjectJsonConverter<string, DxDifficultyType>;
