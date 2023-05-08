using System.Text.Json.Serialization;

namespace Site.Shared.Api.Dto;

public sealed record TokensResponse(
    [property: JsonPropertyName("token")] string Token
);