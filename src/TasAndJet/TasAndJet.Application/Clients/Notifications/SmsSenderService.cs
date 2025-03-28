﻿using Infobip.Api.Client;
using Infobip.Api.Client.Api;
using Infobip.Api.Client.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TasAndJet.Infrastructure.Options;

namespace TasAndJet.Application.Clients.Notification
{
    public class SmsSenderService(ILogger<SmsSenderService> logger, IOptions<SmsOptions> smsOptions) : ISmsSenderService
    {
        private readonly SmsOptions _smsOptions = smsOptions.Value;

        public async Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
        {
            var configuration = new Configuration()
            {
                BasePath = _smsOptions.BaseUrl,
                ApiKey = _smsOptions.ApiKey
            };

            var smsApi = new SmsApi(configuration);

            var smsMessage = new SmsTextualMessage(
                from: _smsOptions.SenderPhone,
                destinations: [new SmsDestination(to: phoneNumber)],
                text: message);

            var smsRequest = new SmsAdvancedTextualRequest(
                messages: [smsMessage]);

            try
            {
                await smsApi.SendSmsMessageAsync(smsRequest, cancellationToken);
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
