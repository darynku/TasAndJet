namespace TasAndJet.Infrastructure.Options
{
    public class SmsOptions
    {
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string Sender { get; set; } = string.Empty;
    }

}
