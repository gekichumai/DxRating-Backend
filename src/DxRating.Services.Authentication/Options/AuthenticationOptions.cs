using DxRating.Common.Utils;
using DxRating.Services.Authentication.Enums;

namespace DxRating.Services.Authentication.Options;

public class AuthenticationOptions
{
    public JwtOptions Jwt { get; set; } = new();

    public List<OpenIdConnectOptions> OpenIdConnect { get; set; } = [];

    public List<OAuthProviderOptions> OAuthProviders { get; set; } = [];
}

public record JwtOptions
{
    private static string RuntimeJwtDefaultKey => RandomUtils.GetRandomAlphaNumericString(32);

    public string Issuer { get; set; } = "https://example.com";

    public string Audience { get; set; } = "https://example.com";

    public string Key { get; set; } = RuntimeJwtDefaultKey; // generate random key by default

    public int AccessTokenExpire { get; set; } = 15; // 15 minutes

    public int RefreshTokenExpire { get; set; } = 10080; // 7 days
}

public record OAuthProviderOptions
{
    public bool Enable { get; set; }

    public OAuthProviderType Type { get; set; }

    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;

    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
}

public record OpenIdConnectOptions
{
    public bool Enable { get; set; }

    public string Name { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Authority { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;

    public string? MetadataAddress { get; set; }

    public string? AuthorizationEndpoint { get; set; }
    public string? TokenEndpoint { get; set; }
    public string? UserInfoEndpoint { get; set; }

    public List<string> Scopes { get; set; } = ["openid", "profile", "email"];

    public ClaimsOptions Claims { get; set; } = new();
}

public class ClaimsOptions
{
    public string Email { get; set; } = "email";

    public string DisplayName { get; set; } = "name";

    public string Identifier { get; set; } = "sub";
}
