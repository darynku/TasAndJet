using CSharpFunctionalExtensions;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Logging;
using SharedKernel.Common.Api;
using TasAndJet.Application.Applications.Services.Accounts.UploadFile;
using TasAndJet.Application.Events;
using TasAndJet.Domain.Entities.Account;
using TasAndJet.Domain.Entities.Services;
using TasAndJet.Infrastructure;
using TasAndJet.Infrastructure.Providers;
using TasAndJet.Infrastructure.Providers.Abstract;

namespace TasAndJet.Application.Applications.Handlers.Accounts.Register;

public class RegisterUserCommandHandler(
    IUploadFileService uploadFileService,
    IValidator<RegisterUserCommand> validator,
    ApplicationDbContext context,
    ILogger<RegisterUserCommandHandler> logger,
    IPublishEndpoint publishEndpoint) : IRequestHandler<RegisterUserCommand, UnitResult<ErrorList>>
{

    public async Task<UnitResult<ErrorList>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        //todo
        // await validator.ValidateAndThrowAsync(request, cancellationToken);
        
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var role = await context.Roles.FirstOrDefaultAsync(role => role.Id == request.RoleId, cancellationToken);

            if (role is null)
                return Errors.General.NotFound(null, "role").ToErrorList();

            var passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Password);

            var avatarUrl = string.Empty;
            
            var user = User.CreateUser(  
                Guid.NewGuid(),
                request.FirstName,
                request.LastName,
                request.Email,
                avatarUrl,
                passwordHash,
                request.PhoneNumber,
                request.Region,
                request.Address,
                role);
            
            await context.Users.AddAsync(user, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            
            if (request.Avatar is not null)
                await uploadFileService.HandleUserAvatar(user.Id, request.Avatar, cancellationToken);
            
            if (role.Name == "Driver")
            {
                var vehiclePhoto = string.Empty;
                
                var vehicle = Vehicle.Create(
                    Guid.NewGuid(),
                    user.Id,
                    request.VehicleType,
                    request.Mark,
                    request.Capacity,
                    vehiclePhoto);
                
                await context.Vehicles.AddAsync(vehicle, cancellationToken);
                user.AddVehicle(vehicle);
                await context.SaveChangesAsync(cancellationToken);
                
                if(request.PhotoUrl is not null)
                    await uploadFileService.HandleVehiclePhoto(user.Id, request.PhotoUrl, cancellationToken);
            }
            
            await transaction.CommitAsync(cancellationToken);
            
            await publishEndpoint.Publish(new UserRegisteredEvent(user.Id, user.PhoneNumber), cancellationToken);
            
            return Result.Success<ErrorList>();
            
        }
        catch (Exception ex)
        {
            logger.LogError("Ошибка при регистрации пользователя: {Email}, {Phone} Ошибка: {Message}",
                request.Email, request.PhoneNumber, ex.Message);
            
            await transaction.RollbackAsync(cancellationToken);

            return Error.Failure("register.user", $"Ошибка при регистрации пользователя: {ex.Message}").ToErrorList();
        }
    }
}