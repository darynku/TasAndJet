using CSharpFunctionalExtensions;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common.Api;
using TasAndJet.Contracts.Response;
using TasAndJet.Infrastructure;
using TasAndJet.Infrastructure.Providers.Abstract;

namespace TasAndJet.Application.Applications.Services.Accounts.Google;

public class GoogleAuthService(ApplicationDbContext context, IJwtProvider jwtProvider) : IGoogleAuthService
{
    public async Task<Result<TokenResponse, Error>> AuthenticateWithGoogle(
        string googleToken,
        CancellationToken cancellationToken = default)
    {
        // Валидация Google-токена
        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(googleToken);
        }
        catch (Exception ex)
        {
            return Error.Conflict("google.auth", ex.Message);
        }

        // Поиск пользователя по email или GoogleId
        var user = await context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == payload.Email || u.GoogleId == payload.Subject, cancellationToken);

        if (user == null)
        {
            return Errors.Authentication.UserNotFound();
        }

        // Пользователь найден - используем существующие данные
        var accessToken = jwtProvider.GenerateAccessToken(user);
        var refreshToken = await jwtProvider.GenerateRefreshToken(user, cancellationToken);
        return new TokenResponse(user.Id, accessToken, refreshToken, user.Role);
    }
}