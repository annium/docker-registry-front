using System;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Site.Shared.Api;
using Site.Shared.Auth;
using Site.Shared.Helpers;
using Site.Shared.Storage;

namespace Site;

public static class Services
{
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