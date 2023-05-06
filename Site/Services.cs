using System;
using Microsoft.Extensions.DependencyInjection;

namespace Site;

public static class Services
{
    public static void Register(this IServiceCollection services)
    {
        // core
        services.AddLogging();

        // web
        services.AddAntDesign();
    }

    public static void Setup(this IServiceProvider provider)
    {
    }
}