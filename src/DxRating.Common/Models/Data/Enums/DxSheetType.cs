using DxRating.Common.Abstract;

namespace DxRating.Common.Models.Data.Enums;

// ReSharper disable InconsistentNaming

public record DxSheetType : ValueObject<string, DxSheetType>
{
    protected DxSheetType(string value) : base(value)
    {
    }

    public static readonly DxSheetType DX = new("dx");
    public static readonly DxSheetType STD = new("std");
    public static readonly DxSheetType UTAGE = new("utage");
    public static readonly DxSheetType UTAGE2P = new("utage2p");
}

public class DxSheetTypeJsonConverter : ValueObjectJsonConverter<string, DxSheetType>;
