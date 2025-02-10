using System.Net.Http.Json;
using TasAndJet.Contracts.Data.Accounts;

namespace TasAndJet.Application.Clients;

public class SmsClient(HttpClient client) : ISmsClient
{
    public async Task SendSmsAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        var data = new SendSmsData
        {
            PhoneNumber = phoneNumber
        };
        
        var response = await client.PostAsJsonAsync("http://localhost:5001/api/Accounts/send-2fa-code", data, cancellationToken);
        
        if(!response.IsSuccessStatusCode || response is null)
            throw new HttpRequestException($"Error sending Sms: {response.StatusCode}");
        
    }
}