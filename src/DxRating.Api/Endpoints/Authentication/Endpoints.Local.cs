using System.Security.Claims;
using DxRating.Api.Endpoints.Authentication.Dto;
using DxRating.Api.Endpoints.Authentication.Enums;
using DxRating.Common.Enums;
using DxRating.Common.Extensions;
using DxRating.Common.Options;
using DxRating.Services.Api.Extensions;
using DxRating.Services.Api.Models;
using DxRating.Services.Authentication.Abstract;
using DxRating.Services.Authentication.Constants;
using DxRating.Services.Authentication.Services;
using DxRating.Services.Authentication.Utils;
using DxRating.Services.Email.Enums;
using DxRating.Services.Email.Services;
using DxRating.Services.Email.Templates;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DxRating.Api.Endpoints.Authentication;

public partial class Endpoints
{
    [EndpointDescription("Register new user account")]
    private static async Task<Results<Ok<UserTokenDto>, BadRequest<ErrorResponse>>> RegisterAsync(
        [FromBody] UserRegisterDto userRegisterDto,
        [FromServices] ICurrentUser currentUser,
        [FromServices] IUserService userService,
        [FromServices] TokenService tokenService,
        [FromServices] SessionService sessionService,
        [FromServices] EmailService emailService,
        [FromServices] IConfiguration configuration)
    {
        // Validate if email is already in use
        var existingUser = await userService.GetUserByEmailAsync(userRegisterDto.Email);
        if (existingUser is not null)
        {
            return ErrorCode.EmailAlreadyInUse.ToResponse().ToBadRequest();
        }

        // Validate password complexity
        var isPasswordComplexEnough = SecurityUtils.VerifyPasswordComplexity(userRegisterDto.Password);
        if (isPasswordComplexEnough is false)
        {
            return ErrorCode.PasswordLowComplexity.ToResponse().ToBadRequest();
        }

        // Create user
        var user = await userService.CreateUserFromLocalAsync(userRegisterDto.Email, userRegisterDto.Password);

        // Prepare and send a confirmation email
        var token = await tokenService.CreateEmailConfirmationTokenAsync(user.UserId);

        // Send email
        var serverOptions = configuration.GetOptions<ServerOptions>("Server");
        var obj = new EmailConfirmation
        {
            CallbackUrl = $"{serverOptions.FrontendUrl}/api/auth/confirm?type={nameof(EmailKind.EmailConfirmation)}&token={token.VerificationToken}",
            ExpireAt = token.ExpireAt
        };
        await emailService.SendAsync(user.Email, EmailKind.EmailConfirmation, obj, currentUser.Language);

        var session = await sessionService.CreateSessionAsync(user, currentUser.UserAgent, currentUser.IpAddress);

        return session.MapToUserTokenDto().ToOk();
    }

    [EndpointDescription("Login user")]
    private static async Task<Results<Ok<LoginNextStepDto>, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> LoginAsync(
        [FromBody] UserLoginDto userLoginDto,
        [FromServices] ICurrentUser currentUser,
        [FromServices] IUserService userService,
        [FromServices] SessionService sessionService,
        [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        // Get user
        var user = await userService.GetUserByEmailAsync(userLoginDto.Email);
        if (user is null)
        {
            return ErrorCode.UserNotFound.ToResponse().ToNotFound();
        }

        // Check Password
        if (string.IsNullOrEmpty(user.Password))
        {
            return ErrorCode.InvalidCredentials.ToResponse().ToBadRequest();
        }
        var verifyPassword = SecurityUtils.VerifyPassword(userLoginDto.Password, user.Password);
        if (verifyPassword is false)
        {
            return ErrorCode.InvalidCredentials.ToResponse().ToBadRequest();
        }

        // Create principle
        var claims = sessionService.GetCommonClaims(user);
        var principle = new ClaimsPrincipal(new ClaimsIdentity(claims));

        // Check MFA
        if (user.MfaEnabled)
        {
            await httpContextAccessor.HttpContext!.SignInAsync(AuthenticationConstants.LocalMfaCookieScheme, principle);
            return new LoginNextStepDto
            {
                NextStep = LoginNextStep.VerifyMfa
            }.ToOk();
        }

        await httpContextAccessor.HttpContext!.SignInAsync(AuthenticationConstants.SessionExchangeCookieScheme, principle);
        return new LoginNextStepDto
        {
            NextStep = LoginNextStep.GetSession
        }.ToOk();
    }
}
