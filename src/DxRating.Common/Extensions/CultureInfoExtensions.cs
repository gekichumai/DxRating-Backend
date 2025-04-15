using System.Globalization;
using DxRating.Common.Enums;

namespace DxRating.Common.Extensions;

public static class CultureInfoExtensions
{
    public static Language ParseLanguage(this CultureInfo cultureInfo)
    {
        var cultureName = cultureInfo.Name;

        // Detect English
        if (cultureName.StartsWith("en", StringComparison.InvariantCultureIgnoreCase))
        {
            return Language.English;
        }

        // Detect Japanese
        if (cultureName.StartsWith("ja", StringComparison.InvariantCultureIgnoreCase))
        {
            return Language.Japanese;
        }

        // Detect Chinese (Simplified)
        if (cultureName.StartsWith("zh-hans", StringComparison.InvariantCultureIgnoreCase))
        {
            return Language.ChineseSimplified;
        }

        // Detect Chinese (Traditional)
        if (cultureName.StartsWith("zh-hant", StringComparison.InvariantCultureIgnoreCase))
        {
            return Language.ChineseTraditional;
        }

        // If it is zh but without -hans or -hant, default to Chinese (Simplified)
        if (cultureName.StartsWith("zh", StringComparison.InvariantCultureIgnoreCase))
        {
            return Language.ChineseSimplified;
        }

        // Default to English if no match is found
        return Language.English;
    }

    public static Language ParseLanguage(string code, bool strict = false)
    {
        return code.ToLowerInvariant() switch
        {
            "en" => Language.English,
            "ja" => Language.Japanese,
            "zh-hans" => Language.ChineseSimplified,
            "zh-hant" => Language.ChineseTraditional,
            _ => strict
                ? throw new ArgumentException($"Unsupported language code: {code}")
                : Language.English
        };
    }

    public static string ToLocaleString(this Language language)
    {
        return language switch
        {
            Language.English => "en",
            Language.Japanese => "ja",
            Language.ChineseSimplified => "zh-hans",
            Language.ChineseTraditional => "zh-hant",
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        };
    }
}
