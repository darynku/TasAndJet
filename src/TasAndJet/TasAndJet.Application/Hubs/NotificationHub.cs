using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using TasAndJet.Domain.Entities;

namespace TasAndJet.Application.Hubs;

public interface INotificationHub
{
    Task ReceiveNotification(Notification message);
    Task ReceiveNotification(string message);
}

public class NotificationHub(ILogger<NotificationHub> logger) : Hub<INotificationHub>
{
    public async Task ReceiveMessage(Notification message)
    {
        await Clients.All.ReceiveNotification(message);
    }

    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        if (Context.User is not null)
        {
            await Clients.All.ReceiveNotification($"Joined {Context.User.Identity?.Name}");
        }

        logger.LogInformation("JoinGroup method called. GroupName: {GroupName} ConnectionId: {ConnectionId}", groupName, Context.ConnectionId);
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        if (Context.User is not null)
        {
            await Clients.Group(groupName)
                .ReceiveNotification($"{Context.User.Identity?.Name} left group {groupName}");
        }

        logger.LogInformation("LeaveGroup method called. GroupName: {GroupName} ConnectionId: {ConnectionId}", groupName, Context.ConnectionId);
    }

    public async Task NotifyGroupAsync(string groupName, Notification message)
    {
        await Clients.Group(groupName).ReceiveNotification(message);
    }

    public async Task SendPrivateMessage(string connectionId, Notification message)
    {
        await Clients.Client(connectionId).ReceiveNotification(message);

        logger.LogInformation("SendPrivateMessage method called. Target ConnectionId: {ConnectionId}", connectionId);
    }

    public async Task BroadcastString(string textMessage)
    {
        await Clients.All.ReceiveNotification(textMessage);

        logger.LogInformation("BroadcastString method called. Message: {Message}", textMessage);
    }
}
