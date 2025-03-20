using TasAndJet.Domain.Entities.Account;

namespace TasAndJet.Contracts.Response;

public record TokenResponse(Guid UserId, string AccessToken, Guid RefreshToken, Role Role);