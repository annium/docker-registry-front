using System;
using Microsoft.Extensions.Configuration;
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
            var config = sp.GetRequiredService<IConfiguration>();
            client.BaseAddress = new Uri(config["registry:uri"]!);
        });
        services.AddHttpClient("registry", (sp, client) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                client.BaseAddress = new Uri(config["registry:uri"]!);
            })
            .AddHttpMessageHandler<AuthMessageHandler>();
        services.AddSingleton<AuthMessageHandler>();

        // shared - auth
        services.AddSingleton<AuthStore>();

        // shared - storage
        services.AddSingleton<CredentialsHelper>();

        // shared - storage 
        services.AddSingleton<LocalStorage>();
    }

    public static void Setup(this IServiceProvider provider)
    {
    }
}