using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using DxRating.Common.Abstract;
using DxRating.Common.Extensions;
using DxRating.Common.Utils;
using DxRating.Database;
using DxRating.Domain.Entities.Identity;
using DxRating.Domain.Enums;
using DxRating.Services.Authentication.Enums;
using DxRating.Services.Authentication.Models;
using DxRating.Services.Authentication.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DxRating.Services.Authentication.Services;

public class TokenService
{
    private readonly DxDbContext _dbContext;
    private readonly TimeProvider _timeProvider;
    private readonly TokenLifetimeOptions _tokenLifetimeOptions;
    private readonly JwtOptions _jwtOptions;

    public TokenService(
        DxDbContext dbContext,
        IConfiguration configuration,
        TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
        _tokenLifetimeOptions = configuration.GetOptions<TokenLifetimeOptions>("TokenLifetime");
        _jwtOptions = configuration.GetOptions<JwtOptions>("Authentication:Jwt");
    }

    public async Task<VerificationTokenDescriptor> CreateEmailConfirmationTokenAsync(Guid userId)
    {
        var verificationToken = RandomUtils.GetRandomAlphaNumericString(32);
        var expireAt = _timeProvider.GetUtcNow().AddSeconds(_tokenLifetimeOptions.EmailConfirmation);

        return await CreateVerificationTokenAsync(userId, TokenType.EmailConfirmation, verificationToken, expireAt);
    }

    public async Task<VerificationTokenDescriptor> CreatePasswordResetTokenAsync(Guid userId)
    {
        var verificationToken = RandomUtils.GetRandomAlphaNumericString(32);
        var expireAt = _timeProvider.GetUtcNow().AddSeconds(_tokenLifetimeOptions.PasswordReset);

        return await CreateVerificationTokenAsync(userId, TokenType.PasswordReset, verificationToken, expireAt);
    }

    public async Task<SessionTokenDescriptor> CreateSessionAsync(Guid userId, string email, string? ua, IPAddress? ipAddress)
    {
        var sessionId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new("id", sessionId.ToString()),
            new("sub", userId.ToString()),
            new("email", email)
        };

        var now = _timeProvider.GetUtcNow();
        var accessTokenExpireAt = now.AddSeconds(_jwtOptions.AccessTokenExpire);
        var refreshTokenExpireAt = now.AddSeconds(_jwtOptions.RefreshTokenExpire);

        var accessToken = GenerateAccessToken(claims, accessTokenExpireAt);
        var refreshToken = RandomUtils.GetRandomAlphaNumericString(32);

        var session = new Session
        {
            Id = sessionId,
            UserId = userId,
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

    public async Task<Result<SessionTokenDescriptor, AuthenticationErrorCode>> RefreshSessionAsync(string currentRefreshToken)
    {
        var now = _timeProvider.GetUtcNow();
        var session = _dbContext.Sessions.FirstOrDefault(x => x.RefreshToken == currentRefreshToken);
        if (session is null || session.RefreshTokenExpiresAt < now)
        {
            return AuthenticationErrorCode.SessionExpired;
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

    private async Task<VerificationTokenDescriptor> CreateVerificationTokenAsync(Guid userId, TokenType tokenType, string verificationToken, DateTimeOffset expireAt)
    {
        var token = new Token
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenType = tokenType,
            VerificationToken = verificationToken,
            ExpiresAt = expireAt
        };

        await _dbContext.Tokens.AddAsync(token);
        await _dbContext.SaveChangesAsync();

        return new VerificationTokenDescriptor
        {
            VerificationToken = verificationToken,
            ExpireAt = expireAt
        };
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
