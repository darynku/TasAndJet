namespace TasAndJet.Infrastructure.Options;

public class StripeOptions
{
    public string BaseUrl { get; init; } = string.Empty;
    public string PublishableKey { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    public string ProductId { get; init; } = string.Empty;
    public string PriceId { get; init; } = string.Empty;
    public string WebhookSecret { get; init; } = string.Empty;
    public string SuccessUrl { get; init; } = string.Empty;
    public string CancelUrl { get; init; } = string.Empty;
}