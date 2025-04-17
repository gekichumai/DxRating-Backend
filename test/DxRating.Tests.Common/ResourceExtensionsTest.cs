using DxRating.Common.Extensions;

namespace DxRating.Tests.Common;

public class ResourceExtensionsTest
{
    [Test]
    public async Task LoadResourcesTest()
    {
        var assembly = typeof(ResourceExtensionsTest).Assembly;

        var testTxt = await assembly.GetResourceStringAsync("Resources.test.txt");

        await Assert.
            That(testTxt)
            .IsEqualTo("""
                       A test resource file

                       """);
    }

    [Test]
    public async Task LoadI18NResourcesTest()
    {
        var assembly = typeof(ResourceExtensionsTest).Assembly;

        var resources = assembly.GetI18NResources("Resources");

        await Assert
            .That(resources)
            .IsNotEmpty()
            .And
            .HasCount(4);
    }
}
