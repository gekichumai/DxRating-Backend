using System.Globalization;
using System.Security.Claims;
using DxRating.Domain.Entities.Identity;
using DxRating.Services.Authentication.Enums;

namespace DxRating.Services.Authentication.Abstract;

public interface ICurrentUser
{
    public Task AuthenticateAsync(string authenticationScheme);
    public Task SignOutAsync(string authenticationScheme);

    public ClaimsPrincipal? Principal { get; set; }
    public bool IsAuthenticated { get; set; }
    public string? AuthenticationScheme { get; set; }
    public IdentityProviderType IdentityProviderType { get; set; }
    public Guid? UserId { get; set; }

    public CultureInfo CultureInfo { get; set;  }

    public Task<User?> GetUserAsync();
}
