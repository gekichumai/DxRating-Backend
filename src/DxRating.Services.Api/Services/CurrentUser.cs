using System.Globalization;
using System.Net;
using System.Security.Claims;
using DxRating.Common.Enums;
using DxRating.Common.Extensions;
using DxRating.Domain.Entities.Identity;
using DxRating.Services.Authentication.Abstract;
using DxRating.Services.Authentication.Constants;
using DxRating.Services.Authentication.Enums;
using DxRating.Services.Authentication.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace DxRating.Services.Api.Services;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserService _userService;

    public CurrentUser(IHttpContextAccessor httpContextAccessor, UserService userService)
    {
        _httpContextAccessor = httpContextAccessor;
        _userService = userService;

        var httpContext = _httpContextAccessor.HttpContext!;

        Principal = httpContext.User;

        IpAddress = httpContext.Connection.RemoteIpAddress;
        UserAgent = httpContext.Request.Headers.TryGetValue("User-Agent", out var ua) ? ua.ToString() : null;

        // If the X-DXRating-Language exist, use it first
        var dxRatingLanguage = httpContext.Request.Headers.TryGetValue("X-DXRating-Language", out var dxRatingLanguageHeader)
            ? dxRatingLanguageHeader.ToString()
            : null;

        // If the X-DXRating-Language does not exist, just use CultureInfo.CurrentCulture
        if (string.IsNullOrEmpty(dxRatingLanguage))
        {
            Language = CultureInfo.CurrentCulture.ParseLanguage();
        }
        else
        {
            try
            {
                Language = CultureInfoExtensions.ParseLanguage(dxRatingLanguage, true);
            }
            catch (Exception)
            {
                // Ignore
                Language = CultureInfo.CurrentCulture.ParseLanguage();
            }
        }
    }

    public ClaimsPrincipal? Principal { get; set; }
    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;
    public Guid? UserId => Guid.TryParse(Principal?.FindFirst("sub")?.Value, out var id) ? id : null;

    public string? AuthenticationScheme => Principal?.FindFirstValue(AuthenticationConstants.AuthenticationSchemeNameClaimType);
    public IdentityProviderType? IdentityProviderType => Enum.TryParse<IdentityProviderType>(
        Principal?.FindFirstValue(AuthenticationConstants.AuthenticationProviderTypeClaimType), out var result)
        ? result
        : null;

    public IPAddress? IpAddress { get; }
    public string? UserAgent { get; }
    public Language Language { get; }

    public async Task<User?> GetUserAsync()
    {
        if (UserId.HasValue is false)
        {
            return null;
        }

        return await _userService.GetUserAsync(UserId.Value);
    }

    public IQueryable<User> GetUserQueryable()
    {
        return _userService.GetUserQueryable();
    }

    public async Task AuthenticateAsync(string authenticationScheme)
    {
        var result = await _httpContextAccessor.HttpContext!.AuthenticateAsync(authenticationScheme);

        Principal = result.Principal;
    }

    public async Task SignOutAsync(string authenticationScheme)
    {
        await _httpContextAccessor.HttpContext!.SignOutAsync(authenticationScheme);
    }
}
