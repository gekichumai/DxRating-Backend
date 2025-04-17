namespace DxRating.Services.Authentication.Constants;

public static class AuthenticationConstants
{
    public const string AuthenticationSchemeNameClaimType = "dxrating/authentication/scheme";
    public const string AuthenticationProviderTypeClaimType = "dxrating/authentication/type";

    public const string SessionExchangeCookieName = ".DX.SessionExchange";
    public const string LocalMfaCookieName = ".DX.LocalMfa";

    public const string SessionExchangeCookieScheme = "session-exchange-cookie";
    public const string LocalMfaCookieScheme = "local-mfa-cookie";
    public const string BearerAuthenticationScheme = "bearer";
}
