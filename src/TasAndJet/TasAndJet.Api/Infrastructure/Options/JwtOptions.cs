namespace TasAndJet.Api.Infrastructure.Options;

public class JwtOptions
{
    public string SecretKey { get; init; } = string.Empty;
    public int Expires { get; init; }
}