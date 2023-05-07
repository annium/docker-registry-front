using System;
using System.Text;
using Site.Shared.Auth;

namespace Site.Shared.Helpers;

public class CredentialsHelper
{
    public string AuthScheme => "Basic";

    public string Encode(string user, string password) => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}"));

    public Credentials Decode(string credentials)
    {
        var parts = Encoding.UTF8.GetString(Convert.FromBase64String(credentials)).Split(':');

        return new Credentials(parts[0], parts[1]);
    }
}