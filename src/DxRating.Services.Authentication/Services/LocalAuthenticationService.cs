using DxRating.Common.Abstract;
using DxRating.Common.Enums;
using DxRating.Common.Enums.Errors;
using DxRating.Common.Utils;
using DxRating.Database;
using DxRating.Domain.Entities.Identity;
using DxRating.Services.Authentication.Abstract;
using DxRating.Services.Authentication.Utils;
using DxRating.Services.Email.Enums;
using DxRating.Services.Email.Services;
using DxRating.Services.Email.Templates;
using Microsoft.EntityFrameworkCore;

namespace DxRating.Services.Authentication.Services;

public class LocalAuthenticationService
{
    private readonly DxDbContext _dbContext;
    private readonly EmailService _emailService;
    private readonly ICurrentUser _currentUser;

    public LocalAuthenticationService(DxDbContext dbContext, EmailService emailService, ICurrentUser currentUser)
    {
        _dbContext = dbContext;
        _emailService = emailService;
        _currentUser = currentUser;
    }

    public async Task<Result<EmptyObject, AuthenticationErrorCode>> CreateUserAsync(string email, string password)
    {
        // Validate if email is already in use
        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
        if (existingUser is not null)
        {
            return AuthenticationErrorCode.EMAIL_ALREADY_EXISTS;
        }

        // Validate password complexity
        var isPasswordComplexEnough = SecurityUtils.VerifyPasswordComplexity(password);
        if (isPasswordComplexEnough)
        {
            return AuthenticationErrorCode.PASSWORD_LOW_COMPLEXITY;
        }

        // Hash password
        var hashedPassword = SecurityUtils.HashPassword(password);

        // Create user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Password = hashedPassword
        };

        // Prepare and send confirmation email
        var token = RandomUtils.GetRandomAlphaNumericString(32);
        user.EmailConfirmationToken = token;

        // Send email
        var obj = new EmailConfirmation
        {
        };
        await _emailService.SendAsync(user.Email, EmailKind.EmailConfirmation, obj, _currentUser.CultureInfo);

        // Save to DB
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        return new EmptyObject();
    }
}
