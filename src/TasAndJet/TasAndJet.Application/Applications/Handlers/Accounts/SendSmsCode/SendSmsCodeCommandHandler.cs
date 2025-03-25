using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using TasAndJet.Application.Clients;
using TasAndJet.Application.Clients.Notification;

namespace TasAndJet.Application.Applications.Handlers.Accounts.SendSmsCode;

public class SendSmsCodeCommandHandler(
    ISmsSenderService smsSenderService, 
    ILogger<SendSmsCodeCommandHandler> logger,
    IDistributedCache cache) : IRequestHandler<SendSmsCodeCommand, bool>
{
    
    public async Task<bool> Handle(SendSmsCodeCommand request, CancellationToken cancellationToken)
    {
        var code = GenerateCode();

        string message = $"Ваш код для двухфакторной аутентификации: {code}";                                                                                                                                                               
        
        try
        {
            await cache.SetStringAsync(request.PhoneNumber, code,
                options: new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)), 
                token: cancellationToken);

            await smsSenderService.SendSmsAsync(request.PhoneNumber, message, cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при отправке SMS: {@Error}", ex.Message);
            throw;
        }
    }

    private string GenerateCode()
    {  
        var random = new Random();
        return random.Next(1000, 9999).ToString();
    }
}