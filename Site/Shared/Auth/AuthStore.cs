using Site.Shared.Helpers;
using Site.Shared.Storage;

namespace Site.Shared.Auth;

public class AuthStore
{
    private const string CredentialsKey = "credentials";
    private const string TokenKey = "token";

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

    public bool HasCredentials()
    {
        return _localStorage.HasKey(CredentialsKey);
    }

    public string? LoadCredentials()
    {
        return _localStorage.TryGetString(CredentialsKey, out var credentials) ? credentials : null;
    }

    public void ClearCredentials()
    {
        _localStorage.Remove(CredentialsKey);
        _localStorage.Remove(TokenKey);
    }

    public void SaveCredentials(string user, string password)
    {
        var credentials = _credentialsHelper.Encode(user, password);

        _localStorage.SetString(CredentialsKey, credentials);
    }

    public string? LoadToken()
    {
        return _localStorage.TryGetString(TokenKey, out var credentials) ? credentials : null;
    }

    public void SaveToken(string token)
    {
        _localStorage.SetString(TokenKey, token);
    }
}