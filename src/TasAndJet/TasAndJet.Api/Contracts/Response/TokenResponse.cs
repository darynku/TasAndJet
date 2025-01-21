using TasAndJet.Api.Entities.Account;

namespace TasAndJet.Api.Contracts.Response;

public record TokenResponse(string AccessToken, Guid RefreshToken, Role Role);