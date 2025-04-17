using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

namespace DxRating.Hosting.Smtp4dev;

public static class Smtp4devResourceBuilderExtension
{
    public static IResourceBuilder<Smtp4devResource> AddSmtp4dev(
        this IDistributedApplicationBuilder builder,
        string name,
        int? httpPort = null,
        int? smtpPort = null)
    {
        var resource = new Smtp4devResource(name);

        return builder.AddResource(resource)
            .WithImage(Smtp4devContainerImageTags.Image)
            .WithImageRegistry(Smtp4devContainerImageTags.Registry)
            .WithImageTag(Smtp4devContainerImageTags.Tag)
            .WithHttpEndpoint(
                targetPort: 80,
                port: httpPort,
                name: Smtp4devResource.HttpEndpointName)
            .WithEndpoint(
                targetPort: 25,
                port: smtpPort,
                name: Smtp4devResource.SmtpEndpointName);
    }

    public static IResourceBuilder<Smtp4devResource> WithDataVolume(this IResourceBuilder<Smtp4devResource> builder, string name)
    {
        return builder.WithVolume(name, "/smtp4dev");
    }
}

internal static class Smtp4devContainerImageTags
{
    internal const string Registry = "docker.io";

    internal const string Image = "rnwood/smtp4dev";

    internal const string Tag = "3.8.3";
}
