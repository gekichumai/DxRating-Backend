using System.Net;
using System.Security.Claims;
using DxRating.Common.Enums;
using DxRating.Domain.Entities.Identity;
using DxRating.Services.Authentication.Enums;

namespace DxRating.Services.Authentication.Abstract;

public interface ICurrentUser
{
    public Guid? UserId { get; }
    public ClaimsPrincipal? Principal { get; set; }
    public bool IsAuthenticated { get; }

    public string? AuthenticationScheme { get; }
    public IdentityProviderType? IdentityProviderType { get; }

    public IPAddress? IpAddress { get; }
    public string? UserAgent { get; }
    public Language Language { get; }

    public Task<User?> GetUserAsync();
    public Task AuthenticateAsync(string authenticationScheme);
}
