using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Pathoschild.Http.Client;
using Site.Shared.Api.Registry;
using Site.Shared.Auth;
using Site.Shared.Helpers;

namespace Site.Shared.Api.Server;

public class ServerApi
{
    private readonly AuthStore _authStore;
    private readonly CredentialsHelper _credentialsHelper;
    private readonly AsyncLazy<ServerConfig> _serverConfig;

    public ServerApi(
        AuthStore authStore,
        CredentialsHelper credentialsHelper,
        IHttpClientFactory httpClientFactory
    )
    {
        _authStore = authStore;
        _credentialsHelper = credentialsHelper;
        _serverConfig = new AsyncLazy<ServerConfig>(GetServerConfig(httpClientFactory.CreateClient("registry")));
    }

    public async Task<bool> LoginAsync(string user, string password)
    {
        var (httpClient, service) = await _serverConfig;

        var response = await httpClient
            .SetAuthentication(_credentialsHelper.AuthScheme, _credentialsHelper.Encode(user, password))
            .GetAsync("v2/token")
            .WithArgument("account", user)
            .WithArgument("service", service);

        if (!response.IsSuccessStatusCode)
            return false;

        var responseData = await response.As<LoginResponse>();

        _authStore.SaveCredentials(user, password);
        _authStore.SaveToken(responseData.Token);

        return true;
    }

    private static Func<Task<ServerConfig>> GetServerConfig(HttpClient registryHttpClient) => async () =>
    {
        var response = await registryHttpClient.GetAsync("/v2/");
        if (response.StatusCode is not HttpStatusCode.Unauthorized)
            throw new InvalidOperationException($"Registry is expected to return {HttpStatusCode.Unauthorized} with auth scheme and server");

        var (server, service, _) = RegistryAuthenticationHelper.Parse(response.Headers);
        var client = new FluentClient(new HttpClient { BaseAddress = server });
        client.SetOptions(ignoreHttpErrors: true);

        return new ServerConfig(client, service);
    };

    private record ServerConfig(FluentClient Client, string Service);
}

file record LoginResponse(
    [property: JsonPropertyName("token")] string Token
);