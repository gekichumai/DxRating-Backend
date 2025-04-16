using System.Text.Json.Serialization;
using DxRating.Services.Authentication.Enums;

namespace DxRating.Api.Endpoints.Authentication.Dto;

public record AuthenticationProviderDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = null!;

    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter<IdentityProviderType>))]
    public IdentityProviderType Type { get; set; }
}
