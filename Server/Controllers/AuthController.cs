using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        ICredentialsReader credentialsReader,
        IScopeReader scopeReader,
        ITokenWriter tokenWriter,
        Configuration config,
        ILogger<AuthController> logger
    )
    {
        _authService = authService;
        _credentialsReader = credentialsReader;
        _scopeReader = scopeReader;
        _tokenWriter = tokenWriter;
        _config = config;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetToken(string account, string service, string scope)
    {
        _logger.LogInformation(
            "Get account={Account} token for service={Service} in scope={Scope}",
            account,
            service,
            scope
        );
        if (string.IsNullOrWhiteSpace(account))
        {
            _logger.LogWarning("Account is not specified");
            return BadRequest("Account must be specified");
        }

        if (string.IsNullOrWhiteSpace(service) || service != _config.Auth.Service)
        {
            _logger.LogWarning(
                "Service={Service} doesn't match configured service={AuthService}",
                service,
                _config.Auth.Service
            );
            return BadRequest("Service must be specified and match configured service value");
        }

        var credentials = _credentialsReader.Read(Request);
        if (credentials is null)
        {
            _logger.LogWarning("Failed to read credentials from request");
            return Unauthorized();
        }

        if (account != credentials.Login)
        {
            _logger.LogInformation(
                "Account={Account} doesn't match credentials login={Login}",
                account,
                credentials.Login
            );
            return BadRequest("Account must match login in credentials");
        }

        var accesses = new List<AccessScope>();
        if (string.IsNullOrWhiteSpace(scope))
        {
            if (!_authService.Login(credentials.Login, credentials.Password))
            {
                _logger.LogInformation(
                    "Credentials login={Login} and password={Password} are invalid",
                    credentials.Login,
                    credentials.Password
                );
                return Unauthorized();
            }
        }
        else
        {
            var accessScope = _scopeReader.Read(scope);
            if (accessScope is null)
            {
                _logger.LogInformation("Access scope is null");
                return BadRequest();
            }

            var allowedActions = _authService.GetAllowedActions(credentials.Login, credentials.Password, accessScope);
            accesses.Add(accessScope with { Actions = allowedActions });
        }

        var token = _tokenWriter.WriteToken(service, account, accesses);
        _logger.LogInformation(
            "Granted access to account={Account} token for service={Service} in scope={Scope}. Access scopes: {Accesses}. Token: {Token}",
            account,
            service,
            scope,
            string.Join(", ", accesses.Select(x => x.ToString())),
            token
        );
        _logger.LogInformation("Granted access");

        return Ok(new { access_token = token, token });
    }
}
