using CSharpFunctionalExtensions;
using Google.Apis.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedKernel.Common;
using TasAndJet.Contracts.Response;
using TasAndJet.Domain.Entities.Account;
using TasAndJet.Infrastructure;
using TasAndJet.Infrastructure.Providers.Abstract;
using GoogleOptions = TasAndJet.Infrastructure.Options.GoogleOptions;

namespace TasAndJet.Application.Applications.Handlers.Accounts.Google;

public class GoogleAuthCommandHandler(
    IOptions<GoogleOptions> googleOptions,
    IJwtProvider jwtProvider,
    ApplicationDbContext context)
    : IRequestHandler<GoogleAuthCommand, Result<TokenResponse, Error>>
{
    private readonly GoogleOptions _googleOptions = googleOptions.Value;

    public async Task<Result<TokenResponse, Error>> Handle(GoogleAuthCommand request, CancellationToken cancellationToken)
    {
        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(request.GoogleToken);
        }
        catch(Exception ex)
        {
            return Error.Conflict("google.auth", ex.Message);
        }

        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == payload.Email, cancellationToken);
        
        var role = await context.Roles.FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken);
        
        if (role is null)
            return Errors.User.InvalidRole();
        
        if (user is null)
        {
            // 🔹 Создаем нового пользователя с указанной ролью
            user = User.CreateGoogleUser(Guid.NewGuid(), payload.GivenName, payload.FamilyName, payload.Email, payload.Subject, role);
            await context.Users.AddAsync(user, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        else if (string.IsNullOrEmpty(user.GoogleId))
        {
            // 🔹 Привязываем Google ID, если его не было
            user.LinkGoogleAccount(payload.Subject);
        }

        await context.SaveChangesAsync(cancellationToken);

        // 🔹 Генерируем JWT
        var token = jwtProvider.GenerateAccessToken(user);
        var refreshToken = await jwtProvider.GenerateRefreshToken(user, cancellationToken);
        var response = new TokenResponse(token, refreshToken, role);

        return response;
    }
}