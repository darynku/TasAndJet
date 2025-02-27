using CSharpFunctionalExtensions;
using MediatR;
using SharedKernel.Common;
using TasAndJet.Contracts.Data.Accounts;

namespace TasAndJet.Application.Applications.Handlers.Accounts.VerifyCode;

public class VerifyCodeCommand : IRequest<UnitResult<ErrorList>>
{
    public VerifyCodeCommand(VerifyCodeData data)
    {
        PhoneNumber = data.PhoneNumber;
        Code = data.Code;
    }
    public string PhoneNumber { get; }
    public string Code { get; }
}