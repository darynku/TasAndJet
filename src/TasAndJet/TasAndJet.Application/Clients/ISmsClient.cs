
namespace TasAndJet.Application.Clients
{
    public interface ISmsClient
    {
        Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
    }
}