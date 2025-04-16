using DxRating.Common.Abstract;
using DxRating.Common.Extensions;
using DxRating.Common.Options;
using DxRating.Services.Authentication.Abstract;
using DxRating.Services.Authentication.Enums;
using DxRating.Services.Authentication.Utils;
using DxRating.Services.Email.Enums;
using DxRating.Services.Email.Services;
using DxRating.Services.Email.Templates;
using Microsoft.Extensions.Configuration;

namespace DxRating.Services.Authentication.Services;

public class LocalAuthenticationService
{
    private readonly TokenService _tokenService;
    private readonly UserService _userService;
    private readonly EmailService _emailService;
    private readonly ICurrentUser _currentUser;
    private readonly ServerOptions _serverOptions;

    public LocalAuthenticationService(
        IConfiguration configuration,
        TokenService tokenService,
        UserService userService,
        EmailService emailService,
        ICurrentUser currentUser)
    {
        _tokenService = tokenService;
        _userService = userService;
        _emailService = emailService;
        _currentUser = currentUser;

        _serverOptions = configuration.GetOptions<ServerOptions>("Server");
    }

    public async Task<Result<EmptyObject, AuthenticationErrorCode>> CreateUserAsync(string email, string password)
    {
        // Validate if email is already in use
        var existingUser = await _userService.GetUserByEmailAsync(email);
        if (existingUser is not null)
        {
            return AuthenticationErrorCode.EmailAlreadyExists;
        }

        // Validate password complexity
        var isPasswordComplexEnough = SecurityUtils.VerifyPasswordComplexity(password);
        if (isPasswordComplexEnough)
        {
            return AuthenticationErrorCode.PasswordLowComplexity;
        }

        // Create user
        var user = await _userService.CreateUserFromLocalAsync(email, password);

        // Prepare and send a confirmation email
        var token = await _tokenService.CreateEmailConfirmationTokenAsync(user.UserId);

        // Send email
        var obj = new EmailConfirmation
        {
            CallbackUrl = $"{_serverOptions.PublicUrl}/api/auth/confirm?type={nameof(EmailKind.EmailConfirmation)}&token={token.VerificationToken}",
            ExpireAt = token.ExpireAt
        };
        await _emailService.SendAsync(user.Email, EmailKind.EmailConfirmation, obj, _currentUser.CultureInfo);

        return new EmptyObject();
    }
}
