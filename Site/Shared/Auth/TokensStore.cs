using System.Collections.Generic;
using System.Linq;
using Site.Shared.Storage;

namespace Site.Shared.Auth;

public class TokensStore
{
    private const string TokenKeyPrefix = "token_";
    private readonly Dictionary<string, string> _tokens;
    private readonly LocalStorage _localStorage;

    public TokensStore(LocalStorage localStorage)
    {
        _localStorage = localStorage;
        var keys = GetKeys();
        _tokens = keys.ToDictionary(x => x, x => _localStorage.GetString(WithPrefix(x)));
    }

    public bool TryGetToken(string scope, out string token) => _tokens.TryGetValue(scope, out token!);

    public void SaveToken(string scope, string token)
    {
        _tokens[scope] = token;
        _localStorage.SetString(WithPrefix(scope), token);
    }

    public void Clear()
    {
        var keys = GetKeys();

        foreach (var key in keys)
            _localStorage.Remove(WithPrefix(key));
    }

    private IReadOnlyCollection<string> GetKeys()
    {
        return _localStorage
            .GetKeys()
            .Where(x => x.StartsWith(TokenKeyPrefix))
            .Select(x => x[TokenKeyPrefix.Length..])
            .ToArray();
    }

    private static string WithPrefix(string key) => $"{TokenKeyPrefix}{key}";
}
