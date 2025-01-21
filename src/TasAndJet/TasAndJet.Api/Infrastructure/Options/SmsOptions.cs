namespace TasAndJet.Api.Infrastructure.Options
{
    public class SmsOptions
    {
        public string BaseUrl { get; init; } = string.Empty;
        public string SenderPhone { get; init; } = string.Empty;
        public string ApiKey { get; init; } = string.Empty;
    }
}
