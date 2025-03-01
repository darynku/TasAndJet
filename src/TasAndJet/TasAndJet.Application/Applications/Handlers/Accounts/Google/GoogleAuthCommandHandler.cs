using CSharpFunctionalExtensions;
using FluentValidation;
using Google.Apis.Auth;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedKernel.Common;
using SharedKernel.Common.Api;
using TasAndJet.Application.Events;
using TasAndJet.Contracts.Response;
using TasAndJet.Domain.Entities.Account;
using TasAndJet.Domain.Entities.Services;
using TasAndJet.Infrastructure;
using TasAndJet.Infrastructure.Providers.Abstract;
using GoogleOptions = TasAndJet.Infrastructure.Options.GoogleOptions;

namespace TasAndJet.Application.Applications.Handlers.Accounts.Google;

public class GoogleAuthCommandHandler(
    IOptions<GoogleOptions> googleOptions,
    IJwtProvider jwtProvider,
    ApplicationDbContext context,
    IPublishEndpoint publishEndpoint,
    IValidator<GoogleAuthCommand> validator)
    : IRequestHandler<GoogleAuthCommand, Result<TokenResponse, Error>>
{
    private readonly GoogleOptions _googleOptions = googleOptions.Value;

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
                payload.FamilyName,
                payload.Email,
                payload.Subject, 
                request.PhoneNumber,
                role);
            
            await context.Users.AddAsync(user, cancellationToken);
            
            if (role.Name == "Driver")  
            {
                var vehicle = Vehicle.Create(
                    Guid.NewGuid(),
                    user.Id,
                    request.VehicleDto.VehicleType,
                    request.VehicleDto.Mark,
                    request.VehicleDto.Capacity,
                    request.VehicleDto.PhotoUrl);

                await context.Vehicles.AddAsync(vehicle, cancellationToken);
                
                user.AddVehicle(vehicle);
                
                await context.SaveChangesAsync(cancellationToken);
            }
        }
        else if (string.IsNullOrEmpty(user.GoogleId))
        {
            // 🔹 Привязываем Google ID, если его не было
            user.LinkGoogleAccount(payload.Subject);
        }

        await context.SaveChangesAsync(cancellationToken);

        if (!user.PhoneConfirmed && string.IsNullOrEmpty(user.PhoneNumber))
        {
            await publishEndpoint.Publish(new UserRegisteredEvent(user.Id, user.PhoneNumber), cancellationToken);
        }
        // 🔹 Генерируем JWT
        var token = jwtProvider.GenerateAccessToken(user);
        var refreshToken = await jwtProvider.GenerateRefreshToken(user, cancellationToken);
        var response = new TokenResponse(token, refreshToken, role);

        return response;
    }
}