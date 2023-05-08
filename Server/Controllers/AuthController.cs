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
    private readonly Configuration _config;

    public AuthController(
        IAuthService authService,
        ICredentialsReader credentialsReader,
        IScopeReader scopeReader,
        ITokenWriter tokenWriter,
        Configuration config
    )
    {
        _authService = authService;
        _credentialsReader = credentialsReader;
        _scopeReader = scopeReader;
        _tokenWriter = tokenWriter;
        _config = config;
    }

    [HttpGet]
    public IActionResult GetToken(
        string account,
        string service,
        string scope
    )
    {
        if (string.IsNullOrWhiteSpace(account))
            return BadRequest("Account must be specified");

        if (string.IsNullOrWhiteSpace(service) || service != _config.Auth.Service)
            return BadRequest("Service must be specified and match configured service value");

        var credentials = _credentialsReader.Read(Request);
        if (credentials is null)
            return Unauthorized();

        if (account != credentials.Login)
            return BadRequest("Account must match login in credentials");

        var accesses = new List<AccessScope>();
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

            var allowedActions = _authService.GetAllowedActions(credentials.Login, credentials.Password, accessScope);
            accesses.Add(accessScope with { Actions = allowedActions });
        }

        var token = _tokenWriter.WriteToken(service, account, accesses);

        return Ok(new { access_token = token, token });
    }
}