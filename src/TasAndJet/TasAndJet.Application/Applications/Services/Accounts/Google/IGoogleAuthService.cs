using CSharpFunctionalExtensions;
using SharedKernel.Common.Api;
using TasAndJet.Contracts.Response;

namespace TasAndJet.Application.Applications.Services.Accounts.Google;

public interface IGoogleAuthService
{
    Task<Result<TokenResponse, Error>> AuthenticateWithGoogle(
        string googleToken,
        CancellationToken cancellationToken = default);
}