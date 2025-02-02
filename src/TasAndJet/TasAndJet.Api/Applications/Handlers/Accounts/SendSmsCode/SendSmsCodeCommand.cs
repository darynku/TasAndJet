using MediatR;
using TasAndJet.Api.Contracts.Data.Accounts;

namespace TasAndJet.Api.Applications.Handlers.Accounts.SendSmsCode;

public class SendSmsCodeCommand : IRequest<bool>
{
    public SendSmsCodeCommand(SendSmsData data)
    {
        PhoneNumber = data.PhoneNumber;
    }
    public string PhoneNumber { get; }
}