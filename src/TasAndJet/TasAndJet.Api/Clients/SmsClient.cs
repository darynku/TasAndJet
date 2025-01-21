using Infobip.Api.Client;
using Infobip.Api.Client.Api;
using Infobip.Api.Client.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using TasAndJet.Api.Infrastructure.Options;

namespace TasAndJet.Api.Clients
{
    public class SmsClient(ILogger<SmsClient> logger, IOptions<SmsOptions> smsOptions) : ISmsClient
    {
        private readonly SmsOptions _smsOptions = smsOptions.Value;

        public async Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken)
        {
            var configuration = new Configuration()
            {
                BasePath = _smsOptions.BaseUrl,
                ApiKey = _smsOptions.ApiKey
            };

            var smsApi = new SmsApi(configuration);

            var smsMessage = new SmsTextualMessage(
                from: _smsOptions.SenderPhone,
                destinations: new List<SmsDestination>
                {
                    new SmsDestination(to: phoneNumber)
                },
                text: message);

            var smsRequest = new SmsAdvancedTextualRequest(
                messages: new List<SmsTextualMessage>
                {
                    smsMessage
                });

            try
            {
                var smsResponse = await smsApi.SendSmsMessageAsync(smsRequest, cancellationToken);
                logger.LogInformation("SMS sent to {PhoneNumber} with message {Message}", phoneNumber, message);
            }
            catch (ApiException ex)
            {
                logger.LogError($"Error sending SMS to {phoneNumber} with message {message}. " +
                    $"Error: {ex.Message}, Code: {ex.ErrorCode}, Headers: {ex.Headers}, Content: {ex.ErrorContent}");

                throw;
            }

        }
    }
}
