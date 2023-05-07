using System.Net.Http;

namespace Site.Shared.Api.Registry;

public class RegistryApi
{
    private readonly HttpClient _httpClient;

    public RegistryApi(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("registry");
    }
}