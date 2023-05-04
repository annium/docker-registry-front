using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Server.Models;

namespace Server.Services;

public interface IScopeReader
{
    public AccessScope? Read(string scope);
}

internal class ScopeReader : IScopeReader
{
    private readonly ILogger<CredentialsReader> _logger;

    public ScopeReader(ILogger<CredentialsReader> logger)
    {
        _logger = logger;
    }

    public AccessScope? Read(string scope)
    {
        try
        {
            var parts = scope.Split(':').ToArray();
            if (parts.Length != 3)
                return null;

            var repository = parts[1];
            var action = RepositoryAction.None;

            foreach (var scopeAction in parts[2].Split(','))
            {
                if (scopeAction == ScopeAction.Push)
                    action |= RepositoryAction.Write;

                if (scopeAction == ScopeAction.Pull)
                    action |= RepositoryAction.Read;
            }

            return new AccessScope(repository, action);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed to read scope");
            return null;
        }
    }
}