using System.ComponentModel;
using DxRating.Api.Endpoints.Authentication.Dto;
using DxRating.Common.Extensions;
using DxRating.Services.Api.Abstract;
using DxRating.Services.Api.Extensions;
using DxRating.Services.Authentication.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using AuthenticationOptions = DxRating.Services.Authentication.Options.AuthenticationOptions;

namespace DxRating.Api.Endpoints.Authentication;

public partial class Endpoints : IEndpointMapper
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var authGroup = endpoints.MapGroup("/auth")
            .HasApiVersion(1)
            .WithTags("Authentication");

        // General
        authGroup.MapGet("/providers", GetProviders);
        authGroup.MapGet("/login/{provider}", InitiateAuthentication);

        // Erc4361
        var erc4361Group = authGroup.MapGroup("/erc4361");
        erc4361Group.MapGet("/challenge", GetErc4361Challenge);
        erc4361Group.MapPost("/verify", VerifyErc4361SignatureAsync);

        // Local
        authGroup.MapPost("/register", RegisterAsync).RequireTurnstile("Register");
        authGroup.MapPost("/login", LoginAsync).RequireTurnstile("Login");
        authGroup.MapPut("/logout", LogoutCurrentSessionAsync).RequireBearerAuth();
        authGroup.MapDelete("/logout/{sessionId:guid}", LogoutSessionAsync).RequireBearerAuth();

        // Session
        var sessionGroup = authGroup.MapGroup("/session");
        sessionGroup.MapGet("/", GetSessionAsync).RequireSessionExchangeCookie();

        // WebAuthn
        var webAuthnGroup = authGroup.MapGroup("/webauthn");
        webAuthnGroup.MapGet("/attestation", AttestationAsync).RequireBearerAuth();
        webAuthnGroup.MapPost("/attestation", AttestationVerifyAsync).RequireBearerAuth();
        webAuthnGroup.MapGet("/assertion", AssertionAsync);
        webAuthnGroup.MapPost("/assertion", AssertionVerifyAsync);
    }

    [EndpointDescription("Get authentication providers")]
    private static Ok<List<AuthenticationProviderDto>> GetProviders(
        [FromServices] IConfiguration configuration)
    {
        return GetAuthenticationProviderList(configuration).ToOk();
    }

    [EndpointDescription("Initiate authentication")]
    private static Results<ChallengeHttpResult, NotFound> InitiateAuthentication(
        [FromServices] IConfiguration configuration,
        [FromRoute(Name = "provider"), Description("OAuth provider name")]
        string provider,
        [FromQuery(Name = "return_url"), Description("The URL that will redirect to after authentication with TokenExchangeCookie set")]
        string returnUrl)
    {
        var providers = GetAuthenticationProviderList(configuration, true);
        if (providers.Any(x => x.Name == provider) is false)
        {
            return TypedResults.NotFound();
        }

        var properties = new AuthenticationProperties
        {
            RedirectUri = returnUrl,
            Items =
            {
                ["LoginProvider"] = provider,
                ["returnUrl"] = returnUrl
            }
        };

        return TypedResults.Challenge(properties, [provider]);
    }

    private static List<AuthenticationProviderDto> GetAuthenticationProviderList(IConfiguration configuration, bool oauth = false)
    {
        var options = configuration.GetOptions<AuthenticationOptions>("Authentication");
        var result = new List<AuthenticationProviderDto>();

        // OpenID Connect
        result.AddRange(options.OpenIdConnect
            .Where(x => x.Enable)
            .Select(oidc => new AuthenticationProviderDto
            {
                Name = oidc.Name,
                DisplayName = oidc.DisplayName,
                Type = IdentityProviderType.OAuth
            }));

        // OAuth
        result.AddRange(options.OAuthProviders
            .Where(x => x.Enable)
            .Select(oAuth => new AuthenticationProviderDto
            {
                Name = oAuth.Name,
                DisplayName = oAuth.DisplayName,
                Type = IdentityProviderType.OAuth
            }));

        if (oauth)
        {
            return result;
        }

        // Local
        result.Add(new AuthenticationProviderDto
        {
            Name = "Local",
            DisplayName = "Local",
            Type = IdentityProviderType.Local
        });

        // WebAuthn
        if (options.WebAuthn.Enable)
        {
            result.Add(new AuthenticationProviderDto
            {
                Name = "webauthn",
                DisplayName = "WebAuthn",
                Type = IdentityProviderType.WebAuthn
            });
        }

        // Erc4361
        if (options.Erc4361.Enable)
        {
            result.Add(new AuthenticationProviderDto
            {
                Name = "erc4361",
                DisplayName = "Erc4361",
                Type = IdentityProviderType.Erc4361
            });
        }

        return result;
    }
}
