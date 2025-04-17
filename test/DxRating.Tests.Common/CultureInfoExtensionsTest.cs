using System.Globalization;
using DxRating.Common.Enums;
using DxRating.Common.Extensions;

namespace DxRating.Tests.Common;

public class CultureInfoExtensionsTest
{
    [Test]
    [Arguments("en", Language.English)]
    [Arguments("en-gb", Language.English)]
    [Arguments("zh", Language.ChineseSimplified)]
    [Arguments("zh-cn", Language.ChineseSimplified)]
    [Arguments("zh-tw", Language.ChineseSimplified)]
    [Arguments("zh-hans", Language.ChineseSimplified)]
    [Arguments("zh-hans-sg", Language.ChineseSimplified)]
    [Arguments("zh-hant", Language.ChineseTraditional)]
    [Arguments("zh-hant-tw", Language.ChineseTraditional)]
    [Arguments("ja", Language.Japanese)]
    [Arguments("ja-jp", Language.Japanese)]
    [Arguments("fr", Language.English)]
    [Arguments("es-es", Language.English)]
    public async Task ParseCultureTest(string culture, Language language)
    {
        var cultureInfo = new CultureInfo(culture);
        var parsedLanguage = cultureInfo.ParseLanguage();

        await Assert
            .That(parsedLanguage)
            .IsEqualTo(language);
    }
}
