using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using DxRating.Common.Abstract;
using DxRating.Common.Enums;
using DxRating.Common.Extensions;
using DxRating.Common.Utils;
using DxRating.Database;
using DxRating.Domain.Entities.Identity;
using DxRating.Services.Authentication.Models;
using DxRating.Services.Authentication.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DxRating.Services.Authentication.Services;

public class SessionService
{
    private readonly DxDbContext _dbContext;
    private readonly TimeProvider _timeProvider;
    private readonly JwtOptions _jwtOptions;

    public SessionService(IConfiguration configuration, TimeProvider timeProvider, DxDbContext dbContext)
    {
        _timeProvider = timeProvider;
        _dbContext = dbContext;
        _jwtOptions = configuration.GetOptions<JwtOptions>("Authentication:Jwt");
    }

    /// <summary>
    /// Creates a new session for the specified user and generates access and refresh tokens.
    /// </summary>
    /// <param name="user">The user whom the session belongs to.</param>
    /// <param name="ua">The user agent string of the client initiating the session. This is optional.</param>
    /// <param name="ipAddress">The IP address of the client initiating the session. This is optional.</param>
    /// <returns>A <see cref="SessionTokenDescriptor"/> contains the session tokens.</returns>
    public async Task<SessionTokenDescriptor> CreateSessionAsync(User user, string? ua = null, IPAddress? ipAddress = null)
    {
        var sessionId = Guid.NewGuid();
        var claims = GetCommonClaims(user);
        claims.Add(new Claim("id", sessionId.ToString()));

        var now = _timeProvider.GetUtcNow();
        var accessTokenExpireAt = now.AddSeconds(_jwtOptions.AccessTokenExpire);
        var refreshTokenExpireAt = now.AddSeconds(_jwtOptions.RefreshTokenExpire);

        var accessToken = GenerateAccessToken(claims, accessTokenExpireAt);
        var refreshToken = RandomUtils.GetRandomAlphaNumericString(32);

        var session = new Session
        {
            Id = sessionId,
            UserId = user.UserId,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = accessTokenExpireAt,
            RefreshTokenExpiresAt = refreshTokenExpireAt,
            UserAgent = ua,
            IpAddress = ipAddress
        };

        await _dbContext.Sessions.AddAsync(session);
        await _dbContext.SaveChangesAsync();

        return new SessionTokenDescriptor
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpireAt = accessTokenExpireAt,
            RefreshTokenExpireAt = refreshTokenExpireAt
        };
    }

    /// <summary>
    /// Refreshes an existing session by validating the provided refresh token, updating access and refresh tokens, and returning a new session descriptor.
    /// </summary>
    /// <param name="currentRefreshToken">The refresh token associated with the session that needs to be refreshed.</param>
    /// <returns>
    /// A <see cref="Result{SessionTokenDescriptor, ErrorCode}"/> containing the updated session tokens if successful,
    /// or an error code indicating the reason for failure (e.g., session expired).
    /// </returns>
    public async Task<Result<SessionTokenDescriptor, ErrorCode>> RefreshSessionAsync(string currentRefreshToken)
    {
        var now = _timeProvider.GetUtcNow();
        var session = _dbContext.Sessions.FirstOrDefault(x => x.RefreshToken == currentRefreshToken);
        if (session is null || session.RefreshTokenExpiresAt < now)
        {
            return ErrorCode.RefreshTokenExpired;
        }

        var accessTokenExpireAt = now.AddSeconds(_jwtOptions.AccessTokenExpire);
        var refreshTokenExpireAt = now.AddSeconds(_jwtOptions.RefreshTokenExpire);
        var accessToken = RefreshAccessToken(session.AccessToken, accessTokenExpireAt);
        var refreshToken = RandomUtils.GetRandomAlphaNumericString(32);

        session.AccessToken = accessToken;
        session.AccessTokenExpiresAt = accessTokenExpireAt;
        session.RefreshToken = refreshToken;
        session.RefreshTokenExpiresAt = refreshTokenExpireAt;

        await _dbContext.SaveChangesAsync();

        return new SessionTokenDescriptor
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpireAt = accessTokenExpireAt,
            RefreshTokenExpireAt = refreshTokenExpireAt
        };
    }

    public async Task LogoutSessionAsync(Guid sessionId)
    {
        var session = await _dbContext.Sessions.FirstOrDefaultAsync(x => x.Id == sessionId);
        if (session is not null)
        {
            _dbContext.Sessions.Remove(session);
            await _dbContext.SaveChangesAsync();
        }
    }

    public List<Claim> GetCommonClaims(User user)
    {
        return
        [
            new Claim("sub", user.UserId.ToString()),
            new Claim("email", user.Email),
            new Claim("email_confirmed", user.EmailConfirmed.ToString().ToLowerInvariant())
        ];
    }

    private string GenerateAccessToken(IEnumerable<Claim> claims, DateTimeOffset expireAt)
    {
        var key = Encoding.UTF8.GetBytes(_jwtOptions.Key);
        var signingKey  = new SymmetricSecurityKey(key);
        var cred = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: _timeProvider.GetUtcNow().UtcDateTime,
            expires: expireAt.UtcDateTime,
            signingCredentials: cred
        );

        var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenStr;
    }

    private string RefreshAccessToken(string accessToken, DateTimeOffset expireAt)
    {
        var key = Encoding.UTF8.GetBytes(_jwtOptions.Key);
        var signingKey  = new SymmetricSecurityKey(key);
        var cred = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var previousToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
        var newToken = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: previousToken.Claims,
            notBefore: _timeProvider.GetUtcNow().UtcDateTime,
            expires: expireAt.UtcDateTime,
            signingCredentials: cred
        );

        var tokenStr = new JwtSecurityTokenHandler().WriteToken(newToken);
        return tokenStr;
    }
}
