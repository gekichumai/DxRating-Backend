using System.Security.Claims;
using System.Text;
using DxRating.Common.Extensions;
using DxRating.Services.Authentication.Constants;
using DxRating.Services.Authentication.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using AuthenticationOptions = DxRating.Services.Authentication.Options.AuthenticationOptions;

namespace DxRating.Services.Authentication.Configurator;

internal static class IdentityConfigurator
{
    internal static void ConfigureIdentity(this IHostApplicationBuilder builder)
    {
        builder.ConfigureAuthentication();
        builder.ConfigureAuthorization();
    }

    private static void ConfigureAuthentication(this IHostApplicationBuilder builder)
    {
        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

        var authenticationOptions = builder.Configuration.GetOptions<AuthenticationOptions>("Authentication");

        var authenticationBuilder = builder.Services.AddAuthentication();

        authenticationBuilder.AddCookie(AuthenticationConstants.CookieAuthenticationScheme, o =>
        {
            o.Cookie.Name = AuthenticationConstants.CookieName;
            o.Cookie.SameSite = SameSiteMode.None;
            o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            o.ExpireTimeSpan = TimeSpan.FromMinutes(5);

            o.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
            o.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            };
        });

        var jwt = authenticationOptions.Jwt;
        var jwtKey = Encoding.UTF8.GetBytes(jwt.Key);
        authenticationBuilder.AddJwtBearer(AuthenticationConstants.BearerAuthenticationScheme, o =>
        {
            o.Audience = jwt.Audience;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidAudience = jwt.Audience,
                ValidIssuer = jwt.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(jwtKey)
            };
        });

        // OpenID Connect
        foreach (var oidc in authenticationOptions.OpenIdConnect)
        {
            if (oidc.Enable is false)
            {
                continue;
            }
            authenticationBuilder.AddOpenIdConnect(oidc.Name, oidc.DisplayName, o =>
            {
                o.SignInScheme = AuthenticationConstants.CookieAuthenticationScheme;

                o.Scope.Clear();
                foreach (var scope in oidc.Scopes)
                {
                    o.Scope.Add(scope);
                }

                o.Authority = oidc.Authority;
                o.ClientId = oidc.ClientId;
                o.ClientSecret = oidc.ClientSecret;

                o.CallbackPath = $"/auth/callback/{oidc.Name}";

                o.GetClaimsFromUserInfoEndpoint = true;

                o.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, oidc.Claims.Identifier);
                o.ClaimActions.MapJsonKey(ClaimTypes.Name, oidc.Claims.DisplayName);
                o.ClaimActions.MapJsonKey(ClaimTypes.Email, oidc.Claims.Email);

                o.ClaimActions.MapCustomJson(AuthenticationConstants.AuthenticationSchemeNameClaimType, _ => oidc.Name);
                o.ClaimActions.MapCustomJson(AuthenticationConstants.AuthenticationProviderTypeClaimType, _ => IdentityProviderType.OAuth.ToString());

                if (string.IsNullOrEmpty(oidc.MetadataAddress) is false)
                {
                    o.MetadataAddress = oidc.MetadataAddress;
                }
                else
                {
                    if (string.IsNullOrEmpty(oidc.AuthorizationEndpoint) ||
                        string.IsNullOrEmpty(oidc.TokenEndpoint) ||
                        string.IsNullOrEmpty(oidc.UserInfoEndpoint))
                    {
                        throw new InvalidOperationException("MetadataAddress or required endpoints are not provided.");
                    }

                    o.Configuration = new OpenIdConnectConfiguration
                    {
                        AuthorizationEndpoint = oidc.AuthorizationEndpoint,
                        TokenEndpoint = oidc.TokenEndpoint,
                        UserInfoEndpoint = oidc.UserInfoEndpoint
                    };
                }
            });
        }

        // OAuth
        foreach (var oauth in authenticationOptions.OAuthProviders)
        {
            if (oauth.Enable is false)
            {
                continue;
            }
            switch (oauth.Type)
            {
                case OAuthProviderType.GitHub:
                    authenticationBuilder.AddGitHub(oauth.Name, oauth.DisplayName, o =>
                    {
                        o.SignInScheme = AuthenticationConstants.CookieAuthenticationScheme;
                        o.ClientId = oauth.ClientId;
                        o.ClientSecret = oauth.ClientSecret;
                        o.CallbackPath = $"/auth/callback/{oauth.Name}";

                        o.ClaimActions.MapCustomJson(AuthenticationConstants.AuthenticationSchemeNameClaimType, _ => oauth.Name);
                        o.ClaimActions.MapCustomJson(AuthenticationConstants.AuthenticationProviderTypeClaimType, _ => IdentityProviderType.OAuth.ToString());
                    });
                    break;
                case OAuthProviderType.Discord:
                    authenticationBuilder.AddDiscord(oauth.Name, oauth.DisplayName, o =>
                    {
                        o.SignInScheme = AuthenticationConstants.CookieAuthenticationScheme;
                        o.ClientId = oauth.ClientId;
                        o.ClientSecret = oauth.ClientSecret;
                        o.CallbackPath = $"/auth/callback/{oauth.Name}";

                        o.ClaimActions.MapCustomJson(AuthenticationConstants.AuthenticationSchemeNameClaimType, _ => oauth.Name);
                        o.ClaimActions.MapCustomJson(AuthenticationConstants.AuthenticationProviderTypeClaimType, _ => IdentityProviderType.OAuth.ToString());
                    });
                    break;
                case OAuthProviderType.Google:
                    authenticationBuilder.AddGoogle(oauth.Name, oauth.DisplayName, o =>
                    {
                        o.SignInScheme = AuthenticationConstants.CookieAuthenticationScheme;
                        o.ClientId = oauth.ClientId;
                        o.ClientSecret = oauth.ClientSecret;
                        o.CallbackPath = $"/auth/callback/{oauth.Name}";

                        o.ClaimActions.MapCustomJson(AuthenticationConstants.AuthenticationSchemeNameClaimType, _ => oauth.Name);
                        o.ClaimActions.MapCustomJson(AuthenticationConstants.AuthenticationProviderTypeClaimType, _ => IdentityProviderType.OAuth.ToString());
                    });
                    break;
                default:
                    throw new InvalidOperationException($"Unknown OAuth provider type: {oauth.Type}");
            }
        }
    }

    private static void ConfigureAuthorization(this IHostApplicationBuilder builder)
    {
        builder.Services.AddAuthorization();

        var authorizationBuilder = builder.Services.AddAuthorizationBuilder();

        authorizationBuilder.AddDefaultPolicy("Default", o =>
        {
            o.AddAuthenticationSchemes(AuthenticationConstants.BearerAuthenticationScheme).RequireAuthenticatedUser();
        });
    }
}
