using MediatR;
using TasAndJet.Contracts.Data.Accounts;

namespace TasAndJet.Application.Applications.Handlers.Accounts.SendSmsCode;

public class SendSmsCodeCommand : IRequest<bool>
{
    public SendSmsCodeCommand(SendSmsData data)
    {
        PhoneNumber = data.PhoneNumber;
    }
    public string PhoneNumber { get; }
}