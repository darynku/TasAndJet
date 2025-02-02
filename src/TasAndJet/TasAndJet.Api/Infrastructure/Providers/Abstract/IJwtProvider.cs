﻿using TasAndJet.Api.Entities.Account;

namespace TasAndJet.Api.Infrastructure.Providers.Abstract;

public interface IJwtProvider
{
    string GenerateAccessToken(User user);
    Task<Guid> GenerateRefreshToken(User user, CancellationToken cancellationToken);
}