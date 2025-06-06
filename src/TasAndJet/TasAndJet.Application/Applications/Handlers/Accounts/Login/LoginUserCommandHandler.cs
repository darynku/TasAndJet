﻿using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.Common.Api;
using TasAndJet.Contracts.Response;
using TasAndJet.Infrastructure;
using TasAndJet.Infrastructure.Providers.Abstract;

namespace TasAndJet.Application.Applications.Handlers.Accounts.Login;

public class LoginUserCommandHandler(
    ApplicationDbContext context, 
    IJwtProvider jwtProvider,
    ILogger<LoginUserCommandHandler> logger) : IRequestHandler<LoginUserCommand, Result<TokenResponse, Error>>
{
    public async Task<Result<TokenResponse, Error>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.PhoneNumber == request.Phone, cancellationToken);

        if (user == null)
            return Error.NotFound("user.not.found","Данного пользователя не существует");
        
        var passwordVerify = BCrypt.Net.BCrypt.EnhancedVerify(request.Password, user.PasswordHash);
        if (!passwordVerify)
            return Errors.User.InvalidCredentials();
        
        logger.LogInformation("Пользователь вошел в систему: {@Phone}", request.Phone);
        
        var accessToken = jwtProvider.GenerateAccessToken(user);
        var refreshToken = await jwtProvider.GenerateRefreshToken(user, cancellationToken);

        var role = user.Role;
        
        return new TokenResponse(user.Id, accessToken, refreshToken, role);
    }
}