
namespace TasAndJet.Application.Clients.Notification
{
    public interface ISmsSenderService
    {
        Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
    }
}