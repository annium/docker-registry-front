using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Pathoschild.Http.Client;
using Site.Shared.Api.Dto;
using Site.Shared.Auth;
using Site.Shared.Helpers;

namespace Site.Shared.Api;

public class AuthMessageHandler : DelegatingHandler
{
    private readonly FluentClient _serverHttpClient;
    private readonly AuthStore _authStore;
    private readonly TokensStore _tokensStore;
    private readonly CredentialsHelper _credentialsHelper;
    private readonly string _service;

    public AuthMessageHandler(
        IHttpClientFactory httpClientFactory,
        AuthStore authStore,
        TokensStore tokensStore,
        CredentialsHelper credentialsHelper,
        IConfiguration config
    )
    {
        _serverHttpClient = new FluentClient(httpClientFactory.CreateClient("server"));
        _serverHttpClient.SetOptions(ignoreHttpErrors: true);
        _authStore = authStore;
        _tokensStore = tokensStore;
        _credentialsHelper = credentialsHelper;
        _service = config["registry:auth:service"] ?? throw new InvalidOperationException("registry service is not set");
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // if scope passed in header and saved in tokens - set Authorization header
        if (request.PullScope(out var scope) && _tokensStore.TryGetToken(scope, out var token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // send request initially
        var baseResponse = await base.SendAsync(request, cancellationToken);

        // any response except Unauthorized - return as is
        if (baseResponse.StatusCode is not HttpStatusCode.Unauthorized)
            return baseResponse;

        // try load credentials and return base response on failure
        var credentials = _authStore.TryLoadCredentials();
        if (credentials is null)
            return baseResponse;

        // try parse scope from baseResponse headers and return base response on failure
        if (!ParseAuthenticationScope(baseResponse.Headers, out scope))
            return baseResponse;

        // request new token
        var authResponse = await _serverHttpClient
            .SetAuthentication(_credentialsHelper.AuthScheme, _credentialsHelper.Encode(credentials.User, credentials.Password))
            .GetAsync("v2/token")
            .WithArgument("account", credentials.User)
            .WithArgument("service", _service)
            .WithArgument("scope", scope);

        // if token request failed - return baseResponse
        if (!authResponse.IsSuccessStatusCode)
            return baseResponse;

        // extract new token, set request header and save to tokens store
        token = (await authResponse.As<TokensResponse>()).Token;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        _tokensStore.SaveToken(scope, token);

        return await base.SendAsync(request, cancellationToken);
    }

    private static bool ParseAuthenticationScope(HttpResponseHeaders headers, out string scope)
    {
        var authentication = headers.WwwAuthenticate.Single().Parameter!
            .Split(',')
            .Select(x => x.Split('=').Select(y => y.Trim('"')).ToArray())
            .ToDictionary(x => x[0], x => x[1]);

        return authentication.TryGetValue("scope", out scope!);
    }
}