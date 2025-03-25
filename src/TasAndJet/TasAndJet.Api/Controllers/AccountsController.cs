using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Common.Api;
using Swashbuckle.AspNetCore.Annotations;
using TasAndJet.Api.Helpers;
using TasAndJet.Api.Requests;
using TasAndJet.Application.Applications.Handlers.Accounts.Get;
using TasAndJet.Application.Applications.Handlers.Accounts.GetPreSignedUrl;
using TasAndJet.Application.Applications.Handlers.Accounts.Google;
using TasAndJet.Application.Applications.Handlers.Accounts.Login;
using TasAndJet.Application.Applications.Handlers.Accounts.Logout;
using TasAndJet.Application.Applications.Handlers.Accounts.RefreshToken;
using TasAndJet.Application.Applications.Handlers.Accounts.Register;
using TasAndJet.Application.Applications.Handlers.Accounts.SendSmsCode;
using TasAndJet.Application.Applications.Handlers.Accounts.UploadFile;
using TasAndJet.Application.Applications.Handlers.Accounts.VerifyCode;
using TasAndJet.Contracts.Data.Accounts;

namespace TasAndJet.Api.Controllers;

[SwaggerTag("Контроллер для работы с аутентификацией и авторизацией")]
public class AccountsController(
    IMediator mediator,
    CookieHelper cookieHelper) : ApplicationController
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
        return Ok(result.IsSuccess);
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginData data, CancellationToken cancellationToken)
    {
        var command = new LoginUserCommand(data);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        var setRefreshCookie = cookieHelper.SetRefreshSessionCookie(result.Value.RefreshToken);
        
        if (setRefreshCookie.IsFailure)
            return setRefreshCookie.Error.ToResponse();
            
        return Ok(result.Value);
    }

    [HttpPost("{userId}/upload-photo")]
    public async Task<IActionResult> UploadPhoto(Guid userId, IFormFile file, CancellationToken cancellationToken)
    {
        await mediator.Send(new UploadUserPhotoCommand(userId, file), cancellationToken);
        return Ok();
    }

    [HttpGet("{userId}/avatar")]
    public async Task<IActionResult> GetAvatar(Guid userId, CancellationToken cancellationToken)
    {
        var request = new GetPreSignedUrlRequest(userId);
        var preSignedUrl = await mediator.Send(request, cancellationToken);
        return Ok(preSignedUrl);
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
        if (result.IsFailure)
        {
            return result.Error.ToResponse();
        }
        return Ok(result.IsSuccess);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var getRefreshSession = cookieHelper.GetRefreshSessionCookie();
        
        if (getRefreshSession.IsFailure)
            return Unauthorized(getRefreshSession.Error);
        
        var result = await mediator.Send(command, cancellationToken);
        
        if(result.IsFailure)
            return result.Error.ToResponse();
        
        var setRefreshCookie = cookieHelper.SetRefreshSessionCookie(result.Value.RefreshToken);
        
        if (setRefreshCookie.IsFailure)
            return setRefreshCookie.Error.ToResponse();
        
        return Ok(result.Value);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutCommand command, CancellationToken cancellationToken)
    {
        var getRefreshSession = cookieHelper.GetRefreshSessionCookie();
        
        if (getRefreshSession.IsFailure)
            return Unauthorized(getRefreshSession.Error);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();
        
        var deleteRefreshCookie = cookieHelper.DeleteRefreshSessionCookie();
        
        if (deleteRefreshCookie.IsFailure)
            return deleteRefreshCookie.Error.ToResponse();

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

    [HttpPost("google-login")]
    public async Task<IActionResult> SignInWithGoogle([FromBody] GoogleAuthRequest request)
    {
        var result = await mediator.Send(new GoogleAuthCommand(request.GoogleToken, request.PhoneNumber,  request.Region, request.Address, request.RoleId, request.VehicleDto));

        return result.IsSuccess 
            ? Ok(result.Value) 
            : BadRequest(result.Error.Message);
    }
}