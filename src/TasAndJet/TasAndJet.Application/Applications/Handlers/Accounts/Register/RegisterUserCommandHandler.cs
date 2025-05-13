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

public sealed class RegisterUserCommandHandler(
    IValidator<RegisterUserCommand> validator,
    ApplicationDbContext context,
    ILogger<RegisterUserCommandHandler> logger,
    IPublishEndpoint publishEndpoint) : IRequestHandler<RegisterUserCommand, UnitResult<ErrorList>>
{

    public async Task<UnitResult<ErrorList>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email || u.PhoneNumber == request.PhoneNumber, cancellationToken);
            
            if (user is not null)
                return Errors.User.UserAlreadyExist().ToErrorList();
            
            var role = await context.Roles.FirstOrDefaultAsync(role => role.Id == request.RoleId, cancellationToken);

            if (role is null)
                return Errors.General.NotFound(null, "role").ToErrorList();

            var passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Password);

            var avatarUrl = string.Empty;
            
            var result = User.CreateUser(  
                id: Guid.NewGuid(),
                firstName: request.FirstName,
                lastName: request.LastName,
                email: request.Email,
                avatarUrl: avatarUrl,
                passwordHash: passwordHash,
                phoneNumber: request.PhoneNumber,
                region: request.Region,
                address: request.Address,
                role: role);
            
            await context.Users.AddAsync(result, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);
            
            await publishEndpoint.Publish(new UserRegisteredEvent(result.Id, result.PhoneNumber), cancellationToken);
            
            return Result.Success<ErrorList>();
            
        }
        catch (Exception ex)
        {
            logger.LogError("Ошибка при регистрации пользователя: {Phone}, {Phone} Ошибка: {Message}",
                request.Email, request.PhoneNumber, ex.Message);
            
            await transaction.RollbackAsync(cancellationToken);

            return Error.Failure("register.user", $"Ошибка при регистрации пользователя: {ex.Message}").ToErrorList();
        }
    }
}