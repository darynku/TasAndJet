using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace TasAndJet.Application.Applications.Handlers.Accounts.VerifyCode;

public class VerifyCodeCommandHandler(
    ILogger<VerifyCodeCommandHandler> logger,
    IDistributedCache cache) : IRequestHandler<VerifyCodeCommand, bool>
{
    public async Task<bool> Handle(VerifyCodeCommand request, CancellationToken cancellationToken)
    {
        var cacheCode = await cache.GetStringAsync(request.PhoneNumber, cancellationToken);
        
        if (string.IsNullOrEmpty(cacheCode) || request.Code != cacheCode)
        {
            return false;
        }
        
        return true;
    }
}