using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Services;

// ReSharper disable InconsistentNaming

namespace Server.Controllers;

[Route("/v2/token")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICredentialsReader _credentialsReader;
    private readonly IScopeReader _scopeReader;
    private readonly ITokenWriter _tokenWriter;

    public AuthController(
        IAuthService authService,
        ICredentialsReader credentialsReader,
        IScopeReader scopeReader,
        ITokenWriter tokenWriter
    )
    {
        _authService = authService;
        _credentialsReader = credentialsReader;
        _scopeReader = scopeReader;
        _tokenWriter = tokenWriter;
    }

    [HttpGet]
    public IActionResult GetToken(
        string account,
        string service,
        string scope
    )
    {
        var credentials = _credentialsReader.Read(Request);
        if (credentials is null)
            return Unauthorized();

        var access = new Dictionary<string, RepositoryAction>();
        if (string.IsNullOrWhiteSpace(scope))
        {
            if (!_authService.Login(credentials.Login, credentials.Password))
                return Unauthorized();
        }
        else
        {
            var accessScope = _scopeReader.Read(scope);
            if (accessScope is null)
                return BadRequest();

            var allowedAction = _authService.GetAccess(credentials.Login, credentials.Password, accessScope.Repository, accessScope.Action);
            access.Add(accessScope.Repository, allowedAction);
        }

        var token = _tokenWriter.WriteToken(service, account, access);

        return Ok(new { access_token = token, token });
    }
}