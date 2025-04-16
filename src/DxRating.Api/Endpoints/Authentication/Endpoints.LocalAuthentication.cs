using DxRating.Api.Endpoints.Authentication.Dto;
using DxRating.Services.Api.Extensions;
using DxRating.Services.Api.Models;
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
    private static async Task<Results<Ok, BadRequest<ErrorResponse>>> PostRegisterAsync(
        [FromBody] UserRegisterDto userRegisterDto,
        [FromServices] LocalAuthenticationService localAuthenticationService)
    {
        var result = await localAuthenticationService.CreateUserAsync(
            userRegisterDto.Email,
            userRegisterDto.Password);

        if (result.IsFail)
        {
            return TypedResults.BadRequest(new ErrorResponse(result.GetFail().ToString()));
        }

        return TypedResults.Ok();
    }
}
