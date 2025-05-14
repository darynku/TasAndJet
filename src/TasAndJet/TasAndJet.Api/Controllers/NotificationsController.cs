using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Paged;
using TasAndJet.Domain.Entities;
using TasAndJet.Infrastructure;

namespace TasAndJet.Api.Controllers;

public record ReadAllNotificationRequest(List<Guid> NotificationId);
public record ReadNotificationRequest(Guid NotificationId);

public record NotificationResponse(
    Guid Id,
    string Title,
    string Message,
    DateTime CreatedAt,
    bool IsRead,
    string Type,
    string? Data
);


public class NotificationsController(ApplicationDbContext context) : ApplicationController
{
    [HttpGet("{userId}/notifications")]
    [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any, VaryByHeader = "User-Agent")]
    public async Task<IActionResult> GetNotifications([FromRoute] Guid userId, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest("Не найден пользователь");
        }

        var user = await context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            return NotFound("Пользователь не найден");
        }
        
        var notifications = await context.Notifications
            .Where(n => n.UserId == user.Id)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationResponse(
                n.Id,
                n.Title,
                n.Message,
                n.CreatedAt,
                n.IsRead,
                n.Type,
                n.Data
            )).ToPagedListAsync(page, pageSize, cancellationToken);
        
        return Ok(notifications);
    }

    [HttpPost("mark-as-read")]
    public async Task<IActionResult> MarkAsRead([FromBody] ReadNotificationRequest request, CancellationToken cancellationToken)
    {
        var notification = await context.Notifications.FirstOrDefaultAsync(n => n.Id == request.NotificationId, cancellationToken);
        if (notification != null)
        {
            notification.IsRead = true;
            await context.SaveChangesAsync(cancellationToken);
        }
        return Ok();
    }
    
    [HttpPost("mark-as-read/all")]
    public async Task<IActionResult> MarkAsReadAll([FromBody] ReadAllNotificationRequest request, CancellationToken cancellationToken)
    {
        if (request.NotificationId.Count == 0)
        {
            return BadRequest("No notification IDs provided.");
        }

        var notifications = await context.Notifications
            .Where(n => request.NotificationId.Contains(n.Id) && !n.IsRead)
            .ToListAsync(cancellationToken);

        if (notifications.Count == 0)
        {
            return NotFound("No unread notifications found for the provided IDs.");
        }

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        await context.SaveChangesAsync(cancellationToken);
        return Ok();
    }
}