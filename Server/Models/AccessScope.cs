namespace Server.Models;

public sealed record AccessScope(string Repository, RepositoryAction Action);