using System;
using System.Text;
using Site.Shared.Helpers;
using Site.Shared.Storage;

namespace Site.Shared.Auth;

public class AuthStore
{
    private const string CredentialsKey = "credentials";
    private readonly CredentialsHelper _credentialsHelper;
    private readonly LocalStorage _localStorage;

    public AuthStore(
        CredentialsHelper credentialsHelper,
        LocalStorage localStorage
    )
    {
        _credentialsHelper = credentialsHelper;
        _localStorage = localStorage;
    }

    public string? LoadCredentials()
    {
        return _localStorage.TryGetString(CredentialsKey, out var credentials) ? credentials : null;
    }

    public void SaveCredentials(string user, string password)
    {
        var credentials = _credentialsHelper.Encode(user, password);

        _localStorage.SetString(CredentialsKey, credentials);
    }
}