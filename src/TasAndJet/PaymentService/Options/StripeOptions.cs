namespace PaymentService.Options;

public class StripeOptions
{
    public string BaseUrl { get; init; } = string.Empty;
    public string PublishableKey { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
}