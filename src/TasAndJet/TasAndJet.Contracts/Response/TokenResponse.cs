using TasAndJet.Domain.Entities.Account;

namespace TasAndJet.Contracts.Response;

public record TokenResponse(string AccessToken, Guid RefreshToken, Role Role);