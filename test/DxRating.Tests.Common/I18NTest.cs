using DxRating.Common.Enums;
using DxRating.Common.Extensions;
using DxRating.Common.Services;

namespace DxRating.Tests.Common;

public class I18NTest
{
    [Test]
    public async Task I18NServiceTest()
    {
        var assembly = typeof(I18NTest).Assembly;
        var resources = assembly.GetI18NResources("Resources");

        var i18N = new I18N(resources);

        // test-key
        await Assert.That(i18N.Get("test-key", Language.English)).IsEqualTo("Hello");
        await Assert.That(i18N.Get("test-key", Language.Japanese)).IsEqualTo("こんにちわ");
        await Assert.That(i18N.Get("test-key", Language.ChineseSimplified)).IsEqualTo("你好 (S)");
        await Assert.That(i18N.Get("test-key", Language.ChineseTraditional)).IsEqualTo("你好 (T)");


        // test.key
        await Assert.That(i18N.Get("test.key", Language.English)).IsEqualTo("World");
        await Assert.That(i18N.Get("test.key", Language.Japanese)).IsEqualTo("世界 (J)");
        await Assert.That(i18N.Get("test.key", Language.ChineseSimplified)).IsEqualTo("世界 (S)");
        await Assert.That(i18N.Get("test.key", Language.ChineseTraditional)).IsEqualTo("世界 (T)");

        // Unknown key
        await Assert.That(i18N.Get("unknown-key", Language.English)).IsEqualTo("unknown-key");
        await Assert.That(i18N.Get("unknown-key", Language.Japanese)).IsEqualTo("unknown-key");
        await Assert.That(i18N.Get("unknown-key", Language.ChineseSimplified)).IsEqualTo("unknown-key");
        await Assert.That(i18N.Get("unknown-key", Language.ChineseTraditional)).IsEqualTo("unknown-key");
    }
}
