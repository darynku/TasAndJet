using TasAndJet.Domain.Entities.Account;

namespace TasAndJet.Domain.Entities;

// Notification.cs
public class Notification
{
    public const string DriverAssigned = "driver_assigned";
    public const string ClientAssigned = "client_assigned";
    public const string NewOrder = "new_order";
    private Notification()
    {
    }

    private Notification(Guid id, Guid userId, string title, string message, DateTime createdAt, string type, string data)
    {
        Id = id;
        UserId = userId;
        Title = title;
        Message = message;
        CreatedAt = createdAt;
        Type = type;
        Data = data;
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }      // Кому адресовано
    public User User { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;
    public string Type { get; set; }      // "driver_assigned", "order_completed" и т.д.
    public string? Data { get; set; }     // Доп. данные (JSON)
    
    public static Notification Create(Guid id, Guid userId, string title, string message, DateTime createdAt, string type, string data)
    {
        return new Notification(id, userId, title, message, createdAt, type, data);
    }
}