using System.Text.Json.Serialization;

namespace Site;

public sealed record Configuration
{
    [JsonPropertyName("registry")]
    public RegistryConfiguration Registry { get; init; } = new();
}

public sealed record RegistryConfiguration
{
    [JsonPropertyName("auth")]
    public RegistryAuthConfiguration Auth { get; init; } = new();
}

public sealed record RegistryAuthConfiguration
{
    [JsonPropertyName("service")]
    public string Service { get; init; } = string.Empty;
}