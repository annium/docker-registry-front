using System.Collections.Generic;
using Server.Models;

namespace Server;

public sealed record Configuration
{
    public AuthConfiguration Auth { get; set; } = new();
    public Dictionary<string, UserConfiguration> Users { get; set; } = new();
}

public sealed record AuthConfiguration
{
    public string Service { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
}

public sealed record UserConfiguration
{
    public string Password { get; set; } = string.Empty;
    public Dictionary<string, string[]> Repositories { get; set; } = new();
}