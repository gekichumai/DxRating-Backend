using DxRating.Api.Endpoints.Authentication.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DxRating.Api.Endpoints.Authentication;

public partial class Endpoints
{
    private static void MapLocalAuthenticationEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/local");

        group.MapPost("/register", PostRegisterAsync);
    }

    [EndpointSummary("Register new user account")]
    private static Ok PostRegisterAsync(
        [FromBody] UserRegisterDto userRegisterDto)
    {
        return TypedResults.Ok();
    }
}
