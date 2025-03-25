using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.Common.Exceptions;
using TasAndJet.Domain.Entities.Orders;
using TasAndJet.Domain.Entities.Services;
using TasAndJet.Infrastructure;

namespace TasAndJet.Application.Applications.Handlers.Orders.Create;

public class CreateOrderCommandHandler(
    ApplicationDbContext context,
    ILogger<CreateOrderCommandHandler> logger) : IRequestHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var client = await context.Users.FirstOrDefaultAsync(u => u.Id == request.ClientId, cancellationToken)
            ?? throw new NotFoundException("Такого пользователя не существует");
        
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var vehicle = await context.Vehicles
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        
        var order = client.CreateOrder(
            Guid.NewGuid(),
            request.Description,
            request.PickupAddress, 
            request.DestinationAddress, 
            request.TotalPrice,
            request.VehicleType,
            request.OrderType);
        
        await context.Orders.AddAsync(order, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    
        logger.LogInformation("Заказ был создан успешно {Id}, {Date}", order.Id, order.OrderDate);
    
        client.AddClientOrder(order);

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        
        return order.Id;
    }
}