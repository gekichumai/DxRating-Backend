using System.Text.Json.Serialization;
using DxRating.Services.Authentication.Enums;

namespace DxRating.Api.Endpoints.Authentication.Dto;

public record AuthenticationProviderDto
{
    /// <summary>
    /// The name of the provider.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// The display name of the provider.
    /// </summary>
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = null!;

    /// <summary>
    /// Identity provider type.
    /// </summary>
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter<IdentityProviderType>))]
    public IdentityProviderType Type { get; set; }
}
