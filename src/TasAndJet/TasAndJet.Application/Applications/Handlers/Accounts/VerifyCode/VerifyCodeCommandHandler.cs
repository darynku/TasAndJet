using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using SharedKernel.Common.Api;

namespace TasAndJet.Application.Applications.Handlers.Accounts.VerifyCode;

public class VerifyCodeCommandHandler(
    IDistributedCache cache,
    IValidator<VerifyCodeCommand> validator) : IRequestHandler<VerifyCodeCommand, UnitResult<ErrorList>>
{
    public async Task<UnitResult<ErrorList>> Handle(VerifyCodeCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        
        var cacheCode = await cache.GetStringAsync(request.PhoneNumber, cancellationToken);
        
        if (string.IsNullOrEmpty(cacheCode) || request.Code != cacheCode)
        {
            return Errors.General.ValueIsInvalid(cacheCode).ToErrorList();
        }
        
        return Result.Success<ErrorList>();
    }
}