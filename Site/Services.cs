using System;
using Microsoft.Extensions.DependencyInjection;
using Site.Shared.Api.Server;
using Site.Shared.Auth;
using Site.Shared.Storage;

namespace Site;

public static class Services
{
    public static void Register(this IServiceCollection services)
    {
        services.AddLogging();
        services.AddAntDesign();

        services.AddSingleton<ServerApi>();
        services.AddSingleton<AuthStore>();
        services.AddSingleton<LocalStorage>();
    }

    public static void Setup(this IServiceProvider provider)
    {
    }
}