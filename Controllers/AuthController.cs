using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StajApi.CQRS;
using StajApi.Features.Auth;
using StajApi.Models;

namespace StajApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ICommandHandler<LoginCommand, AuthResult> _loginHandler;
    private readonly ICommandHandler<RefreshTokenCommand, AuthResult> _refreshHandler;

    public AuthController(
        ICommandHandler<LoginCommand, AuthResult> loginHandler,
        ICommandHandler<RefreshTokenCommand, AuthResult> refreshHandler)
    {
        _loginHandler = loginHandler;
        _refreshHandler = refreshHandler;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _loginHandler.Handle(new LoginCommand
        {
            Username = request.Username,
            Password = request.Password
        });

        if (!result.Success)
        {
            return Unauthorized(new
            {
                success = false,
                message = result.Message
            });
        }

        return Ok(new
        {
            success = true,
            accessToken = result.AccessToken,
            refreshToken = result.RefreshToken,
            expiresInMinutes = 60
        });
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request)
    {
        var result = await _refreshHandler.Handle(new RefreshTokenCommand
        {
            RefreshToken = request.RefreshToken
        });

        if (!result.Success)
        {
            return Unauthorized(new
            {
                message = result.Message
            });
        }

        return Ok(new
        {
            success = true,
            accessToken = result.AccessToken,
            refreshToken = result.RefreshToken
        });
    }
}
