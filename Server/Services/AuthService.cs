using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Server.Models;

namespace Server.Services;

public interface IAuthService
{
    bool Login(string user, string pass);
    RepositoryAction GetAccess(string user, string pass, string repository, RepositoryAction action);
}

internal class AuthService : IAuthService
{
    private readonly Configuration _config;
    private readonly HashAlgorithm _hashAlgorithm = SHA512.Create();

    public AuthService(
        Configuration config
    )
    {
        _config = config;
    }

    public bool Login(string user, string pass)
    {
        return TryLogin(user, pass) is not null;
    }

    public RepositoryAction GetAccess(string user, string pass, string repository, RepositoryAction action)
    {
        var repositories = TryLogin(user, pass);
        if (repositories is null)
            return RepositoryAction.None;

        if (repositories.TryGetValue(repository, out var access))
            return access.HasFlag(action) ? access : RepositoryAction.None;

        if (repositories.TryGetValue("*", out access))
            return access.HasFlag(action) ? access : RepositoryAction.None;

        return RepositoryAction.None;
    }

    private Dictionary<string, RepositoryAction>? TryLogin(string user, string pass)
    {
        if (!_config.Users.TryGetValue(user, out var config))
            return null;

        var passHash = Convert.ToHexString(_hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(pass))).ToLowerInvariant();
        if (passHash != config.Password)
            return null;

        return config.Repositories;
    }
}