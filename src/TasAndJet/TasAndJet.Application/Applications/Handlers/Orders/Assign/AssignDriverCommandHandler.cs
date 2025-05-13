using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.Common.Exceptions;
using TasAndJet.Application.Hubs;
using TasAndJet.Domain.Entities;
using TasAndJet.Domain.Entities.Account;
using TasAndJet.Domain.Entities.Orders;
using TasAndJet.Infrastructure;

namespace TasAndJet.Application.Applications.Handlers.Orders.Assign;

public class AssignDriverCommandHandler(
    ApplicationDbContext context,
    IHubContext<NotificationHub, INotificationHub> hubContext,
    ILogger<AssignDriverCommandHandler> logger) : IRequestHandler<AssignDriverCommand>
{
    public async Task Handle(AssignDriverCommand request, CancellationToken cancellationToken)
    {
        var order = await context.Orders
                        .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken)
                    ?? throw new NotFoundException("Заказ не найден");

        var driver = await context.Users
                         .FirstOrDefaultAsync(o => o.Id == request.DriverId, cancellationToken)
                     ?? throw new NotFoundException("Водитель не найден");

        var clientId = order.ClientId; 

        order.AssignDriver(clientId, request.DriverId);

        driver.AddDriverOrder(order);
        
        await context.SaveChangesAsync(cancellationToken);
        
        await SendNotifications(order, driver, cancellationToken);
    }
    private async Task SendNotifications(Order order, User driver, CancellationToken cancellationToken)
    {
        var clientNotification = Notification.Create(
            Guid.NewGuid(),
            order.ClientId, 
            "Водитель назначен!",
            $"{driver.FirstName} принял ваш заказ", 
            DateTime.UtcNow,
            Notification.Assigned,
            JsonSerializer.Serialize(new { orderId = order.Id }));
        
        await context.Notifications.AddAsync(clientNotification, cancellationToken);

        await context.SaveChangesAsync(cancellationToken); 
        
        var driverNotification = Notification.Create(
            Guid.NewGuid(),
            driver.Id, 
            "Новый заказ!",
            $"Новый заказ {order.Id}!", 
            DateTime.UtcNow,
            Notification.NewOrder,
            JsonSerializer.Serialize(new { orderId = order.Id }));
        
        await context.Notifications.AddAsync(driverNotification, cancellationToken);
        
         logger.LogInformation("Созданы уведомления для клиента - {ClientId}, и водителя - {DriverId}", order.ClientId, driver.Id);
         
        await context.SaveChangesAsync(cancellationToken); 

        await hubContext.Clients.Group(order.ClientId.ToString())
            .ReceiveNotification(clientNotification);

        await hubContext.Clients.Group(driver.Id.ToString())
            .ReceiveNotification(driverNotification);
    }

}