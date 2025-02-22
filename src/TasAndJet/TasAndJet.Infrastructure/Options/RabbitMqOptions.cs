namespace TasAndJet.Infrastructure.Options;

public class RabbitMqOptions
{
    public const string SectionName = "RabbitMqOptions"; 
    public string Host { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}