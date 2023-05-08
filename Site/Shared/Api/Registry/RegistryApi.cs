using System.Net.Http;
using System.Threading.Tasks;
using Pathoschild.Http.Client;
using Site.Shared.Auth;

namespace Site.Shared.Api.Registry;

public class RegistryApi
{
    private readonly AuthStore _authStore;
    private readonly FluentClient _httpClient;

    public RegistryApi(
        IHttpClientFactory httpClientFactory,
        AuthStore authStore
    )
    {
        _authStore = authStore;
        _httpClient = new FluentClient(httpClientFactory.CreateClient("registry"));
    }

    public async Task ListRepositoriesAsync()
    {
        await _httpClient
            .SetBearerAuthentication(_authStore.LoadToken())
            .GetAsync("v2/_catalog");
    }
}