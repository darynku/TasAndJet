using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

namespace TasAndJet.Application.Applications.Handlers.Accounts.VerifyCode;

public class VerifyCodeCommandHandler(IDistributedCache cache) : IRequestHandler<VerifyCodeCommand, UnitResult<ErrorList>>
{
    public async Task<UnitResult<ErrorList>> Handle(VerifyCodeCommand request, CancellationToken cancellationToken)
    {
        var cacheCode = await cache.GetStringAsync(request.PhoneNumber, cancellationToken);
        
        if (string.IsNullOrEmpty(cacheCode) || request.Code != cacheCode)
        {
            return Errors.General.ValueIsInvalid(cacheCode).ToErrorList();
        }
        
        return Result.Success<ErrorList>();
    }
}