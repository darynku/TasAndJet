namespace TasAndJet.Application.Clients;

public interface ISmsClient
{
    Task SendSmsAsync(string phoneNumber, CancellationToken cancellationToken = default);
}