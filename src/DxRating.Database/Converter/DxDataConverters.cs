using DxRating.Common.Models.Data.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DxRating.Database.Converter;

public class DxCategoryTypeConverter() : ValueConverter<DxCategoryType, string>(
    v => v.Value,
    v => DxCategoryType.From(v));

public class DxDifficultyTypeConverter() : ValueConverter<DxDifficultyType, string>(
    v => v.Value,
    v => DxDifficultyType.From(v));

public class DxSheetTypeConverter() : ValueConverter<DxSheetType, string>(
    v => v.Value,
    v => DxSheetType.From(v));

public class DxVersionTypeConverter() : ValueConverter<DxVersionType, string>(
    v => v.Slug,
    v => DxVersionType.FromSlug(v));
