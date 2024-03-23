using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Server.Models;

namespace Server.Services;

public interface IAuthService
{
    bool Login(string user, string pass);
    IReadOnlyCollection<ScopeAction> GetAllowedActions(string user, string pass, AccessScope scope);
}

internal class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly HashAlgorithm _hashAlgorithm = SHA512.Create();
    private readonly IReadOnlyDictionary<string, UserConfig> _users;

    public AuthService(Configuration config, ILogger<AuthService> logger)
    {
        _users = config.Users.ToDictionary(
            x => x.Key,
            x => new UserConfig(
                x.Value.Password,
                x.Value.Repositories.ToDictionary(
                    y => (ScopeName)y.Key,
                    y =>
                        y.Value.Select(z => (ScopeAction)z).Where(ScopeAction.IsKnown).ToArray()
                        as IReadOnlyCollection<ScopeAction>
                )
            )
        );
        _logger = logger;
    }

    public bool Login(string user, string pass)
    {
        return TryLogin(user, pass) is not null;
    }

    public IReadOnlyCollection<ScopeAction> GetAllowedActions(string user, string pass, AccessScope scope)
    {
        var cfg = TryLogin(user, pass);
        if (cfg is null)
            return Array.Empty<ScopeAction>();

        // needed to browse catalog
        if (scope.Type == ScopeType.Registry)
            return new[] { ScopeAction.Any };

        if (scope.Type == ScopeType.Repository)
        {
            if (
                // repository is specified explicitly
                cfg.Repositories.TryGetValue(scope.Name, out var allowedActions)
                ||
                // all repositories access is specified
                cfg.Repositories.TryGetValue(ScopeName.Any, out allowedActions)
            )
                return allowedActions.Intersect(scope.Actions).ToArray();

            return Array.Empty<ScopeAction>();
        }

        _logger.LogError("Unexpected request for allowed actions in scope: {@scope}", scope);

        return Array.Empty<ScopeAction>();
    }

    private UserConfig? TryLogin(string user, string pass)
    {
        if (!_users.TryGetValue(user, out var config))
            return null;

        var passHash = Convert.ToHexString(_hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(pass))).ToLowerInvariant();
        if (passHash != config.Password)
            return null;

        return config;
    }

    private record UserConfig(
        string Password,
        IReadOnlyDictionary<ScopeName, IReadOnlyCollection<ScopeAction>> Repositories
    );
}
