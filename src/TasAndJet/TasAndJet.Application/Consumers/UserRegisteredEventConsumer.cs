using MassTransit;
using Microsoft.EntityFrameworkCore;
using TasAndJet.Application.Clients;
using TasAndJet.Application.Events;
using TasAndJet.Infrastructure;

namespace TasAndJet.Application.Consumers;

public class UserRegisteredEventConsumer(
    ISmsClient smsClient,
    ApplicationDbContext dbContext) : IConsumer<UserRegisteredEvent>
{
    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        if(context.Message.PhoneNumber == String.Empty)
            return;

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == context.Message.UserId)
            ?? throw new Exception("User not found");
        
        user.ConfirmPhone();
        await dbContext.SaveChangesAsync();
        
        await smsClient.SendSmsAsync(context.Message.PhoneNumber);
        
        
    }
}