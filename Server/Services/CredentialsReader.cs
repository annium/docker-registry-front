using System;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Server.Models;

namespace Server.Services;

public interface ICredentialsReader
{
    public Credentials? Read(HttpRequest request);
}

internal class CredentialsReader : ICredentialsReader
{
    private readonly ILogger<CredentialsReader> _logger;

    public CredentialsReader(ILogger<CredentialsReader> logger)
    {
        _logger = logger;
    }

    public Credentials? Read(HttpRequest request)
    {
        try
        {
            var authorization = request.Headers.Authorization.ToString().Split(' ');
            if (authorization.Length != 2 || authorization[0].ToLowerInvariant() != "basic")
                return null;

            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authorization[1])).Split(':');
            if (credentials.Length != 2 || string.IsNullOrWhiteSpace(credentials[0]) || string.IsNullOrWhiteSpace(credentials[1]))
                return null;

            return new Credentials(credentials[0], credentials[1]);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "failed to read credentials");
            return null;
        }
    }
}