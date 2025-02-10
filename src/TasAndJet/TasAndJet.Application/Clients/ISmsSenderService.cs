
namespace TasAndJet.Application.Clients
{
    public interface ISmsSenderService
    {
        Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
    }
}