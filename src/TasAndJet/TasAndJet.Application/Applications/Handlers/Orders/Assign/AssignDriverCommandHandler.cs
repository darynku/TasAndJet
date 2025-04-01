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
    IHubContext<NotificationHub> hubContext,
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
        
        await SendNotifications(order, driver);
    }
    private async Task SendNotifications(Order order, User driver)
    {
        var clientNotification = Notification.CreateInstance(
            Guid.NewGuid(),
            order.ClientId, 
            "Водитель назначен!",
            $"{driver.FirstName} принял ваш заказ", 
            DateTime.UtcNow,
            Notification.Assigned,
            JsonSerializer.Serialize(new { orderId = order.Id }));
        
        context.Notifications.Add(clientNotification);

        await context.SaveChangesAsync(); 
        
        var driverNotification = Notification.CreateInstance(
            Guid.NewGuid(),
            driver.Id, 
            "Новый заказ!",
            $"Новый заказ {order.Id}!", 
            DateTime.UtcNow,
            Notification.NewOrder,
            JsonSerializer.Serialize(new { orderId = order.Id }));
        
        context.Notifications.Add(driverNotification);
        
         logger.LogInformation("Созданы уведомления для клиента - {ClientId}, и водителя - {DriverId}", order.ClientId, driver.Id);
         
        await context.SaveChangesAsync(); 

        await hubContext.Clients.Group(order.ClientId.ToString())
            .SendAsync("ReceiveNotification", clientNotification);

        await hubContext.Clients.Group(driver.Id.ToString())
            .SendAsync("ReceiveNotification", driverNotification);
    }

}