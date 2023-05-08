using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Site.Shared.Api.Dto;

public sealed record CatalogResponse(
    [property: JsonPropertyName("repositories")]
    IReadOnlyList<string> Repositories
);

public sealed record Repository(string Name);

public sealed record TagsResponse(
    [property: JsonPropertyName("tags")] IReadOnlyList<string>? Tags
);

public sealed record RepositoryTag(string Name, string Digest);