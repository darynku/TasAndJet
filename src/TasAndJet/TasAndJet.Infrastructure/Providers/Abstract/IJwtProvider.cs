using TasAndJet.Domain.Entities.Account;

namespace TasAndJet.Infrastructure.Providers.Abstract;

public interface IJwtProvider
{
    string GenerateAccessToken(User user);
    Task<Guid> GenerateRefreshToken(User user, CancellationToken cancellationToken);
}