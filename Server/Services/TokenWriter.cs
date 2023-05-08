using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using Server.Models;
using SimpleBase;

namespace Server.Services;

public interface ITokenWriter
{
    public string WriteToken(
        string service,
        string account,
        IReadOnlyCollection<AccessScope> accesses
    );
}

internal class TokenWriter : ITokenWriter
{
    private readonly Configuration _config;
    private readonly RsaSecurityKey _signingKey;

    public TokenWriter(Configuration config)
    {
        _config = config;
        _signingKey = ReadSigningKey();
    }

    public string WriteToken(
        string service,
        string account,
        IReadOnlyCollection<AccessScope> accesses
    )
    {
        var now = DateTime.UtcNow;
        var expires = now + TimeSpan.FromSeconds(60);

        var header = new JwtHeader(new SigningCredentials(_signingKey, SecurityAlgorithms.RsaSha256));

        var accessesList = accesses
            .Select(x => new
            {
                type = x.Type,
                name = x.Name,
                actions = x.Actions.ToArray()
            })
            .ToArray();

        var claims = new List<Claim>
        {
            new("sub", account),
            new("jti", Guid.NewGuid().ToString()),
            new("access", JsonSerializer.Serialize(accessesList), JsonClaimValueTypes.JsonArray),
        };

        var payload = new JwtPayload(_config.Auth.Issuer, service, claims, now, expires);

        var jwt = new JwtSecurityToken(header, payload);

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);

        return token;
    }

    private static RsaSecurityKey ReadSigningKey()
    {
        var raw = File.ReadAllText("key.pem");

        var rsa = RSA.Create();
        rsa.ImportFromPem(raw.ToCharArray());

        return new RsaSecurityKey(rsa)
        {
            KeyId = GetKid(rsa)
        };

        static string GetKid(RSA rsa)
        {
            var publicKeyInfo = rsa.ExportSubjectPublicKeyInfo();
            var kidHash = SHA256.HashData(publicKeyInfo);
            var kidBase32 = Base32.Rfc4648.Encode(kidHash);
            var chunks = new List<string>();
            for (var i = 0; i < 12; i++)
            {
                chunks.Add(kidBase32[(i * 4)..(i * 4 + 4)]);
            }

            return string.Join(':', chunks);
        }
    }
}