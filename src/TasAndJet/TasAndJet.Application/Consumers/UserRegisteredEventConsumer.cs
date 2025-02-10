using MassTransit;
using TasAndJet.Application.Clients;
using TasAndJet.Contracts.Events;

namespace TasAndJet.Application.Consumers;

public class UserRegisteredEventConsumer(ISmsClient smsClient) : IConsumer<UserRegisteredEvent>
{
    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        if(context.Message.PhoneNumber == String.Empty)
            return;

        await smsClient.SendSmsAsync(context.Message.PhoneNumber);
    }
}