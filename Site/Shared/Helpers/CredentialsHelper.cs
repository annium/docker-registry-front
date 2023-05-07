using System;
using System.Text;

namespace Site.Shared.Helpers;

public class CredentialsHelper
{
    public string AuthScheme => "Basic";
    public string Encode(string user, string password) => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}"));
}