using System;
using System.Collections.Generic;
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

            var type = parts[0];
            var name = parts[1];
            var actions = new List<ScopeAction>();

            foreach (ScopeAction scopeAction in parts[2].Split(','))
                if (ScopeAction.IsKnown(scopeAction))
                    actions.Add(scopeAction);

            return new AccessScope(type, name, actions);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed to read scope");
            return null;
        }
    }
}
