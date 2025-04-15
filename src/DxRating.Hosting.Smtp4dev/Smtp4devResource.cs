using Aspire.Hosting.ApplicationModel;

namespace DxRating.Hosting.Smtp4dev;

public sealed class Smtp4devResource : ContainerResource, IResourceWithConnectionString
{
    public Smtp4devResource(string name) : base(name)
    {
    }

    internal const string SmtpEndpointName = "smtp";
    internal const string HttpEndpointName = "http";

    private EndpointReference? _smtpReference;

    public EndpointReference SmtpEndpoint =>
        _smtpReference ??= new EndpointReference(this, SmtpEndpointName);

    public ReferenceExpression ConnectionStringExpression =>
        ReferenceExpression.Create(
            $"smtp://{SmtpEndpoint.Property(EndpointProperty.HostAndPort)}"
        );
}
