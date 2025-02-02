using MediatR;
using Microsoft.EntityFrameworkCore;
using TasAndJet.Api.Entities.Orders;
using TasAndJet.Api.Entities.Services;
using TasAndJet.Api.Infrastructure;

namespace TasAndJet.Api.Applications.Handlers.Orders.Create;

public class CreateOrderCommandHandler(
    ApplicationDbContext context,
    ILogger<CreateOrderCommandHandler> logger) : IRequestHandler<CreateOrderCommand, Guid>
{
    // на резалт паттерн
    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var client = await context.Users.FirstOrDefaultAsync(u => u.Id == request.ClientId, cancellationToken)
            ?? throw new ArgumentException("Такого пользователя не существует");

        var driver = await context.Users.FirstOrDefaultAsync(d => d.Id == request.DriverId, cancellationToken)
            ?? throw new ArgumentException("Такого пользователя не существует");;
        
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var vehicle = Vehicle.Create(Guid.NewGuid(), request.VehicleType, request.PhotoUrl);
        var service = Service.Create(Guid.NewGuid(), request.Title, request.Cost, vehicle, request.ServiceType);
        var order = Order.Create(
            Guid.NewGuid(),
            client.Id,
            driver.Id,
            request.Description,
            request.PickupAddress, 
            request.DestinationAddress, 
            request.OrderDate,
            request.Status,
            service);
        
        await context.Orders.AddAsync(order, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    
        logger.LogInformation("Заказ был создан успешно {@Id}, {@Date}", order.Id, order.OrderDate);
    
        client.AddClientOrder(order);
        driver.AddDriverOrder(order);

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        
        return order.Id;
    }
}