using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.Common.Exceptions;
using TasAndJet.Application.Applications.Handlers.Orders.Assign;
using TasAndJet.Application.Hubs;
using TasAndJet.Domain.Entities;
using TasAndJet.Domain.Entities.Account;
using TasAndJet.Domain.Entities.Orders;
using TasAndJet.Infrastructure;

namespace TasAndJet.Application.Applications.Handlers.Orders.Drivers;

public class AssignByUseCommandHandler(
    ApplicationDbContext context,
    IHubContext<NotificationHub, INotificationHub> hubContext,
    ILogger<AssignByUseCommandHandler> logger) : IRequestHandler<AssignByUseCommand>
{
    public async Task Handle(AssignByUseCommand request, CancellationToken cancellationToken)
    {
        var order = await context.Orders
                        .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken)
                    ?? throw new NotFoundException("Заказ не найден");

        var client = await context.Users
                         .FirstOrDefaultAsync(o => o.Id == request.ClientId, cancellationToken)
                     ?? throw new NotFoundException("Клиент не найден");

        var clientId = client.Id; 

        order.AssignDriver(clientId, order.DriverId);

        client.AddClientOrder(order);
        
        await context.SaveChangesAsync(cancellationToken);
        
        await SendNotifications(order, client, cancellationToken);
    }
    private async Task SendNotifications(Order order, User client, CancellationToken cancellationToken)
    {
        var driverNotification = Notification.Create(
            Guid.NewGuid(),
            order.ClientId, 
            "Клиент назначен!",
            $"{client.FirstName} принял ваш заказ.", 
            DateTime.UtcNow,
            Notification.ClientAssigned,
            JsonSerializer.Serialize(new { orderId = order.Id }));
        
        await context.Notifications.AddAsync(driverNotification, cancellationToken);

        await context.SaveChangesAsync(cancellationToken); 
        
        var clientNotification = Notification.Create(
            Guid.NewGuid(),
            client.Id, 
            "Вы согласились на аренду!",
            $"Ваш заказ {order.Id}!", 
            DateTime.UtcNow,
            Notification.NewOrder,
            JsonSerializer.Serialize(new { orderId = order.Id }));
        
        await context.Notifications.AddAsync(clientNotification, cancellationToken);
        
        logger.LogInformation("Созданы уведомления для водителя - {ClientId}, и клиента - {ClientId}", order.DriverId, client.Id);
         
        await context.SaveChangesAsync(cancellationToken); 

        await hubContext.Clients.Group(order.ClientId.ToString())
            .ReceiveNotification(clientNotification);

        await hubContext.Clients.Group(client.Id.ToString())
            .ReceiveNotification(driverNotification);
    }

}