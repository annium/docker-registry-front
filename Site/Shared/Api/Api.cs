using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Pathoschild.Http.Client;
using Site.Shared.Api.Dto;
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
        Configuration config
    )
    {
        _serverHttpClient = new FluentClient(httpClientFactory.CreateClient("server"));
        _serverHttpClient.SetOptions(ignoreHttpErrors: true);
        _registryHttpClient = new FluentClient(httpClientFactory.CreateClient("registry"));
        _registryHttpClient.SetOptions(ignoreHttpErrors: true);
        _authStore = authStore;
        _credentialsHelper = credentialsHelper;
        _service = config.Registry.Auth.Service;
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

    public async Task<IReadOnlyList<Repository>> GetCatalogAsync()
    {
        var response = await _registryHttpClient
            .GetAsync("v2/_catalog")
            .InScope("registry:catalog:*")
            .As<CatalogResponse>();

        return response.Repositories.Select(x => new Repository(x)).ToArray();
    }

    public async Task<IReadOnlyList<RepositoryTag>> ListTagsAsync(string repository)
    {
        var response = await _registryHttpClient
            .GetAsync($"v2/{repository}/tags/list")
            .InScope($"repository:{repository}:pull")
            .As<TagsResponse>();
        var responseTags = response.Tags ?? Array.Empty<string>();

        var tags = new List<RepositoryTag>(responseTags.Count);
        foreach (var tag in responseTags)
        {
            var tagResponse = await _registryHttpClient
                .GetAsync($"v2/{repository}/manifests/{tag}")
                .WithHeader("Accept", "application/vnd.docker.distribution.manifest.v2+json")
                .InScope($"repository:{repository}:pull");
            tags.Add(new RepositoryTag(tag, tagResponse.Message.Headers.GetValues("Docker-Content-Digest").Single()));
        }

        return tags;
    }

    public async Task<bool> DeleteTagAsync(string repository, string manifest)
    {
        var response = await _registryHttpClient
            .DeleteAsync($"v2/{repository}/manifests/{manifest}")
            .InScope($"repository:{repository}:push");

        return response.IsSuccessStatusCode;
    }
}