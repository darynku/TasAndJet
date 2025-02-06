using System.Security.Authentication;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TasAndJet.Contracts.Response;
using TasAndJet.Infrastructure;
using TasAndJet.Infrastructure.Providers.Abstract;

namespace TasAndJet.Application.Applications.Handlers.Accounts.Login;

public class LoginUserCommandHandler(
    ApplicationDbContext context, 
    IJwtProvider jwtProvider,
    ILogger<LoginUserCommandHandler> logger) : IRequestHandler<LoginUserCommand, TokenResponse>
{
    public async Task<TokenResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null)
            throw new AuthenticationException("Данного пользователя не существует");
        
        var passwordVerify = BCrypt.Net.BCrypt.EnhancedVerify(request.Password, user.PasswordHash);
        if (!passwordVerify)
            throw new AuthenticationException("Неправильный пароль");
        
        
        logger.LogInformation("Пользователь вошел в систему: {@Email}", request.Email);
        
        var accessToken = jwtProvider.GenerateAccessToken(user);
        var refreshToken = await jwtProvider.GenerateRefreshToken(user, cancellationToken);

        var role = user.Role;
        
        return new TokenResponse(accessToken, refreshToken, role);
    }
}