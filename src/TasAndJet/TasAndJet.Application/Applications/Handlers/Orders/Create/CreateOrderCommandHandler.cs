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
    // на резалт паттерн
    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var client = await context.Users.FirstOrDefaultAsync(u => u.Id == request.ClientId, cancellationToken)
            ?? throw new NotFoundException("Такого пользователя не существует");

        var driver = await context.Users.FirstOrDefaultAsync(d => d.Id == request.DriverId, cancellationToken)
            ?? throw new NotFoundException("Такого пользователя не существует");;
        
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        
        //TODO
        var vehicle = Vehicle.Create(Guid.NewGuid(), driver.Id, request.VehicleType, request.Mark, request.Capacity, request.PhotoUrl);
        
        var service = Service.Create(Guid.NewGuid(), request.Title, vehicle, request.ServiceType);
        var order = Order.Create(
            Guid.NewGuid(),
            client.Id,
            driver.Id,
            request.Description,
            request.PickupAddress, 
            request.DestinationAddress, 
            request.OrderDate,
            service);
        
        await context.Orders.AddAsync(order, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    
        logger.LogInformation("Заказ был создан успешно {Id}, {Date}", order.Id, order.OrderDate);
    
        client.AddClientOrder(order);
        driver.AddDriverOrder(order);

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        
        return order.Id;
    }
}