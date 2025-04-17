using DxRating.Common.Extensions;
using DxRating.Common.Utils;
using DxRating.Database;
using DxRating.Domain.Entities.Identity;
using DxRating.Domain.Enums;
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

    public TokenService(
        DxDbContext dbContext,
        IConfiguration configuration,
        TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
        _tokenLifetimeOptions = configuration.GetOptions<TokenLifetimeOptions>("TokenLifetime");
    }

    /// <summary>
    /// Generates an email confirmation token for a user. The token is used to verify the user's email address and has a specific expiration period.
    /// </summary>
    /// <param name="userId">The unique identifier of the user for whom the email confirmation token is being generated.</param>
    /// <returns>A <see cref="VerificationTokenDescriptor"/> containing the email confirmation token and its expiration details.</returns>
    public async Task<VerificationTokenDescriptor> CreateEmailConfirmationTokenAsync(Guid userId)
    {
        var verificationToken = RandomUtils.GetRandomAlphaNumericString(32);
        var expireAt = _timeProvider.GetUtcNow().AddSeconds(_tokenLifetimeOptions.EmailConfirmation);

        return await CreateVerificationTokenAsync(userId, TokenType.EmailConfirmation, verificationToken, expireAt);
    }

    /// <summary>
    /// Generates a password reset token for a user. The token is used to reset the user's password and has a specific expiration period.
    /// </summary>
    /// <param name="userId">The unique identifier of the user for whom the password-reset token is being generated.</param>
    /// <returns>A <see cref="VerificationTokenDescriptor"/> containing the password reset token and its expiration details.</returns>
    public async Task<VerificationTokenDescriptor> CreatePasswordResetTokenAsync(Guid userId)
    {
        var verificationToken = RandomUtils.GetRandomAlphaNumericString(32);
        var expireAt = _timeProvider.GetUtcNow().AddSeconds(_tokenLifetimeOptions.PasswordReset);

        return await CreateVerificationTokenAsync(userId, TokenType.PasswordReset, verificationToken, expireAt);
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
}
