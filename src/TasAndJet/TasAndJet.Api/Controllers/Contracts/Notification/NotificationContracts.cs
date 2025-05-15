namespace TasAndJet.Api.Controllers.Contracts.Notification;

public record NotificationResponse(
    Guid Id,
    string Title,
    string Message,
    DateTime CreatedAt,
    bool IsRead,
    string Type,
    string? Data
);

public record ReadNotificationRequest(Guid NotificationId);

public record ReadAllNotificationRequest(List<Guid> NotificationId);
