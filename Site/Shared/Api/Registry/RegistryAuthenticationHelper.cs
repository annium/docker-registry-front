using System;
using System.Linq;
using System.Net.Http.Headers;

namespace Site.Shared.Api.Registry;

public static class RegistryAuthenticationHelper
{
    public static (Uri Server, string Service, string? Scope) Parse(HttpResponseHeaders headers)
    {
        var authentication = headers.WwwAuthenticate.Single().Parameter!
            .Split(',')
            .Select(x => x.Split('=').Select(y => y.Trim('"')).ToArray())
            .ToDictionary(x => x[0], x => x[1]);

        var realm = new Uri(authentication["realm"]);
        var server = new Uri(realm.GetLeftPart(UriPartial.Authority));

        return (server, authentication["service"], authentication.TryGetValue("scope", out var scope) ? scope : null);
    }
}