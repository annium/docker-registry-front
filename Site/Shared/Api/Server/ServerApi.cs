using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Pathoschild.Http.Client;
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
        _serverConfig = new AsyncLazy<ServerConfig>(GetServerHttpClient(httpClientFactory.CreateClient("registry")));
    }

    public async Task<Response<string>> LoginAsync(string user, string password)
    {
        var (httpClient, service) = await _serverConfig;

        var response = await httpClient
            .SetAuthentication(_credentialsHelper.AuthScheme, _credentialsHelper.Encode(user, password))
            .GetAsync("v2/token")
            .WithArgument("account", user)
            .WithArgument("service", service);

        if (!response.IsSuccessStatusCode)
            return new Response<string>(false, string.Empty);

        var responseData = await response.As<LoginResponse>();

        return new Response<string>(true, responseData.Token);
    }

    private static Func<Task<ServerConfig>> GetServerHttpClient(HttpClient registryHttpClient) => async () =>
    {
        var response = await registryHttpClient.GetAsync("/v2/");
        if (response.StatusCode is not HttpStatusCode.Unauthorized)
            throw new InvalidOperationException($"Registry is expected to return {HttpStatusCode.Unauthorized} with auth scheme and server");

        var authentication = response.Headers.WwwAuthenticate.Single().Parameter!
            .Split(',')
            .Select(x => x.Split('=').Select(x => x.Trim('"')).ToArray())
            .ToDictionary(x => x[0], x => x[1]);

        var realm = new Uri(authentication["realm"]);
        var baseAddress = new Uri(realm.GetLeftPart(UriPartial.Authority));
        var client = new FluentClient(new HttpClient { BaseAddress = baseAddress });

        return new ServerConfig(client, authentication["service"]);
    };

    private record ServerConfig(FluentClient Client, string Service);
}

file record LoginResponse(
    [property: JsonPropertyName("token")] string Token
);