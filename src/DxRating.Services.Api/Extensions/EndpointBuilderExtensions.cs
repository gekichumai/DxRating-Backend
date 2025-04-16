using DxRating.Services.Api.Filters;
using DxRating.Services.Api.Models;
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
}
