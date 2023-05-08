using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using Pathoschild.Http.Client;

namespace Site.Shared.Api;

public static class RequestScopeExtensions
{
    private const string ScopeHeader = "x-registry-scope";

    public static IRequest InScope(this IRequest request, string scope)
    {
        request.Message.Headers.Add(ScopeHeader, scope);

        return request;
    }

    public static bool PullScope(this HttpRequestMessage message, [NotNullWhen(true)] out string? scope)
    {
        if (message.Headers.TryGetValues(ScopeHeader, out var scopes))
        {
            scope = scopes.Single();
            return true;
        }

        scope = null;

        return false;
    }
}