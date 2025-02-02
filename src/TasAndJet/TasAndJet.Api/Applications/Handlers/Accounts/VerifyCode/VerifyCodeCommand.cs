using MediatR;
using TasAndJet.Api.Contracts.Data.Accounts;

namespace TasAndJet.Api.Applications.Handlers.Accounts.VerifyCode;

public class VerifyCodeCommand : IRequest<bool>
{
    public VerifyCodeCommand(VerifyCodeData data)
    {
        PhoneNumber = data.PhoneNumber;
        Code = data.Code;
    }
    public string PhoneNumber { get; }
    public string Code { get; }
}