using DxRating.Api.Endpoints.Authentication.Dto;
using DxRating.Domain.Entities.Identity;
using DxRating.Services.Authentication.Models;
using Riok.Mapperly.Abstractions;

namespace DxRating.Api.Endpoints.Authentication;

[Mapper]
public static partial class Mapper
{
    public static partial UserTokenDto MapToUserTokenDto(this SessionTokenDescriptor session);
}
