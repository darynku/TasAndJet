namespace TasAndJet.Infrastructure.Options;

public class GoogleOptions
{
    public const string SectionName = "GoogleOptions";
    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
}