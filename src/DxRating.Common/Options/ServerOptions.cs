namespace DxRating.Common.Options;

public record ServerOptions
{
    public string PublicUrl { get; set; } = "https://localhost:7293";

    public string FrontendUrl { get; set; } = "http://localhost:3000";

    public string DefaultEmailDomain { get; set; } = "example.com";
}
