using System;
using Microsoft.Extensions.DependencyInjection;

namespace Server;

internal static class Services
{
    public static void Register(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddCors();
        services.AddMvc();
    }

    public static void Setup(this IServiceProvider provider)
    {
    }
}