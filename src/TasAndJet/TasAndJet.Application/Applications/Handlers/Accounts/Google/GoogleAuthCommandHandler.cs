using CSharpFunctionalExtensions;
using FluentValidation;
using Google.Apis.Auth;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedKernel.Common;
using SharedKernel.Common.Api;
using TasAndJet.Application.Applications.Services.Accounts.UploadFile;
using TasAndJet.Application.Events;
using TasAndJet.Contracts.Response;
using TasAndJet.Domain.Entities.Account;
using TasAndJet.Domain.Entities.Services;
using TasAndJet.Infrastructure;
using TasAndJet.Infrastructure.Providers.Abstract;
using GoogleOptions = TasAndJet.Infrastructure.Options.GoogleOptions;

namespace TasAndJet.Application.Applications.Handlers.Accounts.Google;

public class GoogleAuthCommandHandler(
    IJwtProvider jwtProvider,
    ApplicationDbContext context,
    IPublishEndpoint publishEndpoint,
    IValidator<GoogleAuthCommand> validator)
    : IRequestHandler<GoogleAuthCommand, Result<TokenResponse, Error>>
{
    public async Task<Result<TokenResponse, Error>> Handle(GoogleAuthCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        
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
            user = User.CreateGoogleUser(
                Guid.NewGuid(),
                payload.GivenName, 
                payload.FamilyName ?? string.Empty,
                payload.Email,
                payload.Picture,
                payload.Subject, 
                request.PhoneNumber,
                request.Region,
                request.Address,
                role);
            
            await context.Users.AddAsync(user, cancellationToken);
        }
        else if (string.IsNullOrEmpty(user.GoogleId))
        {
            //Привязываем Google ID, если его не было
            user.LinkGoogleAccount(payload.Subject);
        }

        await context.SaveChangesAsync(cancellationToken);

        if (!user.PhoneConfirmed && string.IsNullOrEmpty(user.PhoneNumber))
        {
            await publishEndpoint.Publish(new UserRegisteredEvent(user.Id, user.PhoneNumber), cancellationToken);
        }
        
        var token = jwtProvider.GenerateAccessToken(user);
        var refreshToken = await jwtProvider.GenerateRefreshToken(user, cancellationToken);
        var response = new TokenResponse(user.Id, token, refreshToken, role);

        return response;
    }
}