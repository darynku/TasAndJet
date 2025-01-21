using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using TasAndJet.Api.Applications.Handlers.Login;
using TasAndJet.Api.Applications.Handlers.Register;
using TasAndJet.Api.Clients;
using TasAndJet.Api.Contracts.Data;

namespace TasAndJet.Api.Controllers;

public class AccountController(
    IMediator mediator,
    ISmsClient smsClient,
    IDistributedCache cache) : ApplicationController
{
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterData data, CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(data);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginData data, CancellationToken cancellationToken)
    {
        var command = new LoginUserCommand(data);
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("send-2fa-code")]
    public async Task<IActionResult> SendTwoFactorCode([FromBody] string phoneNumber)
    {
      
        var verificationCode = GenerateVerificationCode();
        await cache.SetStringAsync(phoneNumber, verificationCode, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });

        var message = $"Your verification code is {verificationCode}.";
        await smsClient.SendSmsAsync(phoneNumber, message, CancellationToken.None);

        return Ok("Verification code sent.");
    }

    [HttpPost("verify-2fa")]
    public async Task<IActionResult> VerifyTwoFactorCode([FromBody] string code, [FromHeader] string phoneNumber)
    {
        // Получение кода из кэша
        var storedCode = await cache.GetStringAsync(phoneNumber);

        if (storedCode == null || storedCode != code)
        {
            return Unauthorized("Invalid or expired verification code.");
        }
        await cache.RemoveAsync(phoneNumber);
        return Ok("Authentication successful.");
    }

    private string GenerateVerificationCode()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString(); // Генерация случайного 6-значного кода
    }
}