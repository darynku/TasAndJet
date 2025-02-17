using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using SharedKernel.Common;
using TasAndJet.Application.Applications.Handlers.Accounts.Get;
using TasAndJet.Application.Applications.Handlers.Accounts.Google;
using TasAndJet.Application.Applications.Handlers.Accounts.Login;
using TasAndJet.Application.Applications.Handlers.Accounts.RefreshToken;
using TasAndJet.Application.Applications.Handlers.Accounts.Register;
using TasAndJet.Application.Applications.Handlers.Accounts.SendSmsCode;
using TasAndJet.Application.Applications.Handlers.Accounts.VerifyCode;
using TasAndJet.Contracts.Data.Accounts;

namespace TasAndJet.Api.Controllers;

public class AccountsController(IMediator mediator,
    IHttpClientFactory httpClientFactory) : ApplicationController
{
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterData data, CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(data);
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return result.Error.ToResponse();
        }
        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginData data, CancellationToken cancellationToken)
    {
        var command = new LoginUserCommand(data);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("send-2fa-code")]
    public async Task<IActionResult> SendTwoFactorCode([FromBody] SendSmsData data, CancellationToken cancellationToken)
    {
        var command = new SendSmsCodeCommand(data);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("verify-2fa")]
    public async Task<IActionResult> VerifyTwoFactorCode([FromBody] VerifyCodeData data, CancellationToken cancellationToken)
    {
        var command = new VerifyCodeCommand(data);
        var result = await mediator.Send(command, cancellationToken);
        if (result is false)
        {
            return Unauthorized("Неправильный код или имтек срок действия");
        }
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if(result.IsFailure)
            return result.Error.ToResponse();
        return Ok(result.Value);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        return Ok();
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("google")]
    public async Task<ActionResult> GoogleLogin([FromBody] GoogleAuthCommand command,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    [HttpGet("signin-google")]
    public IActionResult Callback()
    {
        return Ok("Success");
    }

    [HttpPost("google-login-mediatr")]
    public async Task<IActionResult> SignInWithGoogle([FromBody] GoogleAuthRequest request)
    {
        var result = await mediator.Send(new GoogleAuthCommand(request.GoogleToken, request.PhoneNumber, request.RoleId));

        return result.IsSuccess 
            ? Ok(result.Value) 
            : BadRequest(result.Error.Message);
    }

    public record GoogleAuthRequest(string GoogleToken, string PhoneNumber, int RoleId);
}