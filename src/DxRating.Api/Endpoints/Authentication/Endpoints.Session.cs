using System.Security.Claims;
using DxRating.Api.Endpoints.Authentication.Dto;
using DxRating.Common.Enums;
using DxRating.Common.Extensions;
using DxRating.Common.Options;
using DxRating.Domain.Entities.Identity;
using DxRating.Services.Api.Extensions;
using DxRating.Services.Api.Models;
using DxRating.Services.Authentication.Abstract;
using DxRating.Services.Authentication.Constants;
using DxRating.Services.Authentication.Enums;
using DxRating.Services.Authentication.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DxRating.Api.Endpoints.Authentication;

public partial class Endpoints
{
    [EndpointDescription("Create new session")]
    private static async Task<Results<Ok<UserTokenDto>, BadRequest<ErrorResponse>>> GetSessionAsync(
        [FromServices] ICurrentUser currentUser,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IUserService userService,
        [FromServices] SessionService sessionService,
        [FromServices] IConfiguration configuration)
    {
        var providerName = currentUser.AuthenticationScheme;

        if (string.IsNullOrEmpty(providerName) ||
            currentUser.IdentityProviderType is not (IdentityProviderType.Local or IdentityProviderType.OAuth))
        {
            return ErrorCode.InvalidAuthenticationScheme.ToResponse().ToBadRequest();
        }

        // Get Claims
        var sub = currentUser.Principal?.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? currentUser.Principal?.FindFirstValue(ClaimTypes.Name)
                  ?? currentUser.Principal?.FindFirstValue("sub")
                  ?? throw new SecurityTokenException("Subject claim not found");
        var email = currentUser.Principal?.FindFirstValue(ClaimTypes.Email)
                    ?? currentUser.Principal?.FindFirstValue("email")
                    ?? string.Empty;

        var httpContext = httpContextAccessor.HttpContext!;

        // If it is local authentication, create and return the token
        {
            if (currentUser.IdentityProviderType is IdentityProviderType.Local)
            {
                var user = await currentUser.GetUserAsync();
                var session = await sessionService.CreateSessionAsync(user!, currentUser.UserAgent, currentUser.IpAddress);
                await SignOutSessionExchangeCookieCookie(httpContext);
                return session.MapToUserTokenDto().ToOk();
            }
        }

        // Try to get user by using social login
        {
            var user = await userService.GetUserBySocialLoginAsync(providerName, sub);
            if (user is not null)
            {
                await SignOutSessionExchangeCookieCookie(httpContext);
                var session = await sessionService.CreateSessionAsync(user, currentUser.UserAgent, currentUser.IpAddress);
                return session.MapToUserTokenDto().ToOk();
            }
        }

        // Try to get user by using email
        {
            var user = await userService.GetUserByEmailAsync(email);
            if (user is not null)
            {
                await userService.AddSocialLoginAsync(user.UserId, providerName, sub);

                await SignOutSessionExchangeCookieCookie(httpContext);

                var session = await sessionService.CreateSessionAsync(user, currentUser.UserAgent, currentUser.IpAddress);
                return session.MapToUserTokenDto().ToOk();
            }
        }

        // Try to authenticate the user using Bearer token
        {
            await currentUser.AuthenticateAsync(AuthenticationConstants.BearerAuthenticationScheme);
            var user = await currentUser.GetUserAsync();
            if (currentUser.IsAuthenticated && user is not null)
            {
                await userService.AddSocialLoginAsync(user.UserId, providerName, sub);

                await SignOutSessionExchangeCookieCookie(httpContext);

                var session = await sessionService.CreateSessionAsync(user, currentUser.UserAgent, currentUser.IpAddress);
                return session.MapToUserTokenDto().ToOk();
            }
        }

        // Create a new user
        {
            var userId = Guid.NewGuid();
            var serverConfig = configuration.GetOptions<ServerOptions>("Server");
            email = string.IsNullOrEmpty(email) ? $"{providerName}-{sub}@{serverConfig.DefaultEmailDomain}" : email;

            var user = await userService.CreateUserFromExternalAsync(userId, email,
                socialLogins: [
                    new SocialLogin
                    {
                        ConnectionId = Guid.NewGuid(),
                        UserId = userId,
                        Platform = providerName,
                        Identifier = sub
                    }
                ]);

            await SignOutSessionExchangeCookieCookie(httpContext);

            var session = await sessionService.CreateSessionAsync(user, currentUser.UserAgent, currentUser.IpAddress);
            return session.MapToUserTokenDto().ToOk();
        }
    }

    [EndpointDescription("Refresh session using refresh token")]
    private static async Task<Results<Ok<UserTokenDto>, BadRequest<ErrorResponse>>> RefreshSessionAsync(
        [FromBody] RefreshSessionDto refreshSessionDto,
        [FromServices] SessionService sessionService)
    {
        var refreshResult = await sessionService.RefreshSessionAsync(refreshSessionDto.RefreshToken);
        if (refreshResult.IsFail)
        {
            return refreshResult.GetFail().ToResponse().ToBadRequest();
        }

        var session = refreshResult.GetOk();
        return session.MapToUserTokenDto().ToOk();
    }

    [EndpointDescription("Logout current session")]
    private static async Task<EmptyHttpResult> LogoutCurrentSessionAsync(
        [FromServices] ICurrentUser currentUser,
        [FromServices] SessionService sessionService,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        var sessionIdStr = currentUser.Principal?.FindFirstValue("id");
        if (string.IsNullOrEmpty(sessionIdStr))
        {
            return TypedResults.Empty;
        }

        var canParse = Guid.TryParse(sessionIdStr, out var sessionId);
        if (canParse is false)
        {
            return TypedResults.Empty;
        }

        await sessionService.LogoutSessionAsync(sessionId);

        return TypedResults.Empty;
    }


    [EndpointDescription("Logout a specific session")]
    private static async Task<EmptyHttpResult> LogoutSessionAsync(
        [FromRoute] Guid sessionId,
        [FromServices] SessionService sessionService)
    {
        await sessionService.LogoutSessionAsync(sessionId);
        return TypedResults.Empty;
    }

    private static async Task SignOutSessionExchangeCookieCookie(HttpContext httpContext)
    {
        await httpContext.SignOutAsync(AuthenticationConstants.SessionExchangeCookieScheme);
        var cookieNames = httpContext.Request.Cookies
            .Select(x => x.Key)
            .Where(x => x.StartsWith(AuthenticationConstants.SessionExchangeCookieName, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
        foreach (var name in cookieNames)
        {
            httpContext.Response.Cookies.Delete(name, new CookieOptions
            {
                SameSite = SameSiteMode.None,
                Secure = true,
                HttpOnly = true
            });
        }
    }
}
