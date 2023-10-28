using System;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Server.Services;

namespace Server;

internal static class ServicePack
{
    public static void Register(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddCors();
        services.AddMvc();

        // services
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<ICredentialsReader, CredentialsReader>();
        services.AddSingleton<IScopeReader, ScopeReader>();
        services.AddSingleton<ITokenWriter, TokenWriter>();

        // configuration
        services.AddSingleton(
            JsonSerializer.Deserialize<Configuration>(
                File.ReadAllText("config.json"),
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
            )!
        );
    }

    public static void Setup(this IServiceProvider provider)
    {
        // request token writer to ensure private key is present and valid
        provider.GetRequiredService<ITokenWriter>();
    }
}
