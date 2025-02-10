using TasAndJet.Domain.Entities.Account;

namespace TasAndJet.Contracts.Response;

public record LoginResponse(
    string AccessToken,
    Guid RefreshToken,
    Guid UserId,
    Role Role);