namespace TasAndJet.Infrastructure.Options;

public class RabbitMqOptions
{
    public string Host { get; init; } = String.Empty;
    public string Username { get; init; } = String.Empty;
    public string Password { get; init; } = String.Empty;
}