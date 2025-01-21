
namespace TasAndJet.Api.Clients
{
    public interface ISmsClient
    {
        Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken);
    }
}