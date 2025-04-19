using DxRating.Services.Api.Filters;
using DxRating.Services.Api.Models;
using DxRating.Services.Authentication.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace DxRating.Services.Api.Extensions;

public static class EndpointBuilderExtensions
{
    public static RouteHandlerBuilder RequireTurnstile(this RouteHandlerBuilder builder, string action)
    {
        return builder
            .AddEndpointFilter<TurnstileFilter>()
            .WithMetadata(new TurnstileMetadata
            {
                Action = action
            });
    }

    public static RouteHandlerBuilder RequireSessionExchangeCookie(this RouteHandlerBuilder builder)
    {
        return builder.RequireAuthorization(AuthenticationConstants.SessionExchangePolicy);
    }

    public static RouteHandlerBuilder RequireLocalMfaCookie(this RouteHandlerBuilder builder)
    {
        return builder.RequireAuthorization(AuthenticationConstants.LocalMfaPolicy);
    }

    public static RouteHandlerBuilder RequireBearerAuth(this RouteHandlerBuilder builder)
    {
        return builder.RequireAuthorization(AuthenticationConstants.BearerAuthenticationPolicy);
    }
}
