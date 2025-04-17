using DxRating.Api.Endpoints.Authentication.Dto;
using DxRating.Services.Api.Extensions;
using DxRating.Services.Api.Models;
using DxRating.Services.Authentication.Abstract;
using DxRating.Services.Authentication.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DxRating.Api.Endpoints.Authentication;

public partial class Endpoints
{
    private static void MapLocalAuthenticationEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/register", PostRegisterAsync).RequireTurnstile("Register");
    }

    [EndpointDescription("Register new user account")]
    private static async Task<Results<Ok<UserTokenDto>, BadRequest<ErrorResponse>>> PostRegisterAsync(
        [FromBody] UserRegisterDto userRegisterDto,
        [FromServices] ICurrentUser currentUser,
        [FromServices] LocalAuthenticationService localAuthenticationService,
        [FromServices] SessionService sessionService)
    {
        var registerResult = await localAuthenticationService.CreateUserAsync(
            userRegisterDto.Email,
            userRegisterDto.Password);

        if (registerResult.IsFail)
        {
            return registerResult.GetFail().ToResponse().ToBadRequest();
        }

        var user = registerResult.GetOk();
        var session = await sessionService.CreateSessionAsync(user);

        return new UserTokenDto
        {
            AccessToken = session.AccessToken,
            RefreshToken = session.RefreshToken,
            AccessTokenExpiresAt = session.AccessTokenExpireAt,
            RefreshTokenExpiresAt = session.RefreshTokenExpireAt
        }.ToOk();
    }
}
