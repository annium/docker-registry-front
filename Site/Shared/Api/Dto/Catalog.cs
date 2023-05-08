using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Site.Shared.Api.Dto;

public sealed record Catalog(
    [property: JsonPropertyName("repositories")]
    IReadOnlyList<string> Repositories
);

public sealed record Repository(string Name);