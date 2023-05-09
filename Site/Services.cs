using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Site.Shared.Api;
using Site.Shared.Auth;
using Site.Shared.Helpers;
using Site.Shared.Storage;

namespace Site;

public static class Services
{
    public static async Task Configure(this WebAssemblyHostBuilder builder)
    {
        var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
        var config = await http.GetFromJsonAsync<Configuration>("config.json")
            ?? throw new InvalidOperationException("Failed to load config.json");
        builder.Services.AddSingleton(config);
    }

    public static void Register(this IServiceCollection services)
    {
        services.AddLogging();
        services.AddAntDesign();

        // shared - api
        services.AddSingleton<Api>();
        services.AddHttpClient("server", (sp, client) =>
        {
            var env = sp.GetRequiredService<IWebAssemblyHostEnvironment>();
            client.BaseAddress = new Uri(env.BaseAddress);
        });
        services.AddHttpClient("registry", (sp, client) =>
            {
                var env = sp.GetRequiredService<IWebAssemblyHostEnvironment>();
                client.BaseAddress = new Uri(env.BaseAddress);
            })
            .AddHttpMessageHandler<AuthMessageHandler>();
        services.AddSingleton<AuthMessageHandler>();

        // shared - auth
        services.AddSingleton<AuthStore>();
        services.AddSingleton<TokensStore>();

        // shared - storage
        services.AddSingleton<CredentialsHelper>();

        // shared - storage 
        services.AddSingleton<LocalStorage>();
    }

    public static void Setup(this IServiceProvider provider)
    {
    }
}