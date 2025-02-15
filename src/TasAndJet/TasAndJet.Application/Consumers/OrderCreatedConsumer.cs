using MassTransit;
using Microsoft.EntityFrameworkCore;
using TasAndJet.Domain.Events;
using TasAndJet.Infrastructure;

namespace TasAndJet.Application.Consumers;

public class OrderCreatedConsumer(PaymentService.Services.StripeService stripeService) : IConsumer<OrderCreatedEvent>
{
    public Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        return Task.CompletedTask;
    }
}
