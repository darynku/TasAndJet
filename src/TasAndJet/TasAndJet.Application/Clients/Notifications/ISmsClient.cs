namespace TasAndJet.Application.Clients.Notification;

public interface ISmsClient
{
    Task SendSmsAsync(string phoneNumber, CancellationToken cancellationToken = default);
}