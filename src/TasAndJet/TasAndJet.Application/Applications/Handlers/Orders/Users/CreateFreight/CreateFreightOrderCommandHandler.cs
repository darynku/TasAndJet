using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.Common.Exceptions;
using TasAndJet.Infrastructure;

namespace TasAndJet.Application.Applications.Handlers.Orders.CreateFreight;

public class CreateFreightOrderCommandHandler(
    ApplicationDbContext context,
    ILogger<CreateFreightOrderCommandHandler> logger,
    IValidator<CreateFreightOrderCommand> validator) : IRequestHandler<CreateFreightOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateFreightOrderCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAsync(request, cancellationToken);
        var client = await context.Users
                         .FirstOrDefaultAsync(u => u.Id == request.ClientId, cancellationToken)
                     ?? throw new NotFoundException("Такого пользователя не существует");

        var orderId = Guid.NewGuid();

        var order = client.CreateFreightOrder(
            orderId,
            request.Description,
            request.PickupAddress,
            request.DestinationAddress,
            request.CargoWeight,
            request.CargoType,
            request.TotalPrice,
            request.VehicleType,
            request.City
        );

        await context.Orders.AddAsync(order, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Грузовой заказ создан {Id}", order.Id);

        client.AddClientOrder(order);
        await context.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}