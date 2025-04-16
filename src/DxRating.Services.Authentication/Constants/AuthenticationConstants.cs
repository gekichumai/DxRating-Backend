namespace DxRating.Services.Authentication.Constants;

public static class AuthenticationConstants
{
    public const string AuthenticationSchemeNameClaimType = "dxrating/authentication/scheme";
    public const string AuthenticationProviderTypeClaimType = "dxrating/authentication/type";

    public const string CookieName = ".DX.Cookie";

    public const string CookieAuthenticationScheme = "cookie";
    public const string BearerAuthenticationScheme = "bearer";
}
