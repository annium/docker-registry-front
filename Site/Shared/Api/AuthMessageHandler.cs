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
    private readonly CredentialsHelper _credentialsHelper;
    private readonly string _service;

    public AuthMessageHandler(
        IHttpClientFactory httpClientFactory,
        AuthStore authStore,
        CredentialsHelper credentialsHelper,
        IConfiguration config
    )
    {
        _serverHttpClient = new FluentClient(httpClientFactory.CreateClient("server"));
        _authStore = authStore;
        _credentialsHelper = credentialsHelper;
        _service = config["registry:auth:service"] ?? throw new InvalidOperationException("registry service is not set");
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var baseResponse = await base.SendAsync(request, cancellationToken);

        if (baseResponse.StatusCode is not HttpStatusCode.Unauthorized)
            return baseResponse;

        var credentials = _authStore.TryLoadCredentials();
        if (credentials is null)
            return baseResponse;

        var scope = ParseAuthenticationScope(baseResponse.Headers);

        var authResponse = await _serverHttpClient
            .SetAuthentication(_credentialsHelper.AuthScheme, _credentialsHelper.Encode(credentials.User, credentials.Password))
            .GetAsync("v2/token")
            .WithArgument("account", credentials.User)
            .WithArgument("service", _service)
            .WithArgument("scope", scope);

        if (!authResponse.IsSuccessStatusCode)
            return baseResponse;

        var authResponseData = await authResponse.As<TokensResponse>();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authResponseData.Token);

        return await base.SendAsync(request, cancellationToken);
    }

    private static string? ParseAuthenticationScope(HttpResponseHeaders headers)
    {
        var authentication = headers.WwwAuthenticate.Single().Parameter!
            .Split(',')
            .Select(x => x.Split('=').Select(y => y.Trim('"')).ToArray())
            .ToDictionary(x => x[0], x => x[1]);

        return authentication.TryGetValue("scope", out var scope) ? scope : null;
    }
}