using System;
using System.Text;
using Site.Shared.Storage;

namespace Site.Shared.Auth;

public class AuthStore
{
    private const string CredentialsKey = "credentials";
    private readonly LocalStorage _localStorage;

    public AuthStore(LocalStorage localStorage)
    {
        _localStorage = localStorage;
    }

    public string? LoadCredentials()
    {
        return _localStorage.TryGetString(CredentialsKey, out var credentials) ? credentials : null;
    }

    public void SaveCredentials(string login, string password)
    {
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"));

        _localStorage.SetString(CredentialsKey, credentials);
    }
}