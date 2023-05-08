using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Pathoschild.Http.Client;
using Site.Shared.Auth;
using Site.Shared.Helpers;

namespace Site.Shared.Api;

public class Api
{
    private readonly FluentClient _serverHttpClient;
    private readonly FluentClient _registryHttpClient;
    private readonly AuthStore _authStore;
    private readonly CredentialsHelper _credentialsHelper;
    private readonly string _service;

    public Api(
        IHttpClientFactory httpClientFactory,
        AuthStore authStore,
        CredentialsHelper credentialsHelper,
        IConfiguration config
    )
    {
        _serverHttpClient = new FluentClient(httpClientFactory.CreateClient("server"));
        _registryHttpClient = new FluentClient(httpClientFactory.CreateClient("registry"));
        _authStore = authStore;
        _credentialsHelper = credentialsHelper;
        _service = config["registry:auth:service"] ?? throw new InvalidOperationException("registry service is not set");
    }

    public async Task<bool> LoginAsync(string user, string password)
    {
        var response = await _serverHttpClient
            .SetAuthentication(_credentialsHelper.AuthScheme, _credentialsHelper.Encode(user, password))
            .GetAsync("v2/token")
            .WithArgument("account", user)
            .WithArgument("service", _service);

        if (!response.IsSuccessStatusCode)
            return false;

        _authStore.SaveCredentials(user, password);

        return true;
    }

    public async Task ListRepositoriesAsync()
    {
        await _registryHttpClient
            .GetAsync("v2/_catalog");
    }
}