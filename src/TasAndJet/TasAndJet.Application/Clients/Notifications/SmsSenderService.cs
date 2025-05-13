using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TasAndJet.Application.Clients.Notification;
using TasAndJet.Infrastructure.Options;

namespace TasAndJet.Application.Clients.Notifications;
public class SmsSenderService : ISmsSenderService
{
    private readonly SmsOptions _smsOptions;
    private readonly ILogger<SmsSenderService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public SmsSenderService(ILogger<SmsSenderService> logger, IOptions<SmsOptions> smsOptions, IHttpClientFactory httpClientFactory)
    {
        _smsOptions = smsOptions.Value;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        var httpClient = _httpClientFactory.CreateClient();

        // Параметры для запроса
        var smsData = new
        {
            login = _smsOptions.Login,   // Логин от SMSC
            psw = _smsOptions.Password,  // Пароль от SMSC
            sender = _smsOptions.Sender,
            phones = phoneNumber,        // Номер телефона получателя
            mes = message                // Текст сообщения
        };

        var jsonContent = JsonSerializer.Serialize(smsData);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        try
        {
            var response = await httpClient.PostAsync($"{_smsOptions.BaseUrl}/rest/send/", content, cancellationToken);
            response.EnsureSuccessStatusCode(); // Проверка успешности запроса

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation("SMS sent to {PhoneNumber} with message {Message}. Response: {Response}", 
                phoneNumber, message, responseJson);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending SMS to {phoneNumber} with message {message}. Error: {ex.Message}");

            throw;
        }
    }
}