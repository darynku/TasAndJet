using CSharpFunctionalExtensions;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;
using SharedKernel.Validators;
using TasAndJet.Application.Events;
using TasAndJet.Domain.Entities.Account;
using TasAndJet.Infrastructure;

namespace TasAndJet.Application.Applications.Handlers.Accounts.Register;

public class RegisterUserCommandHandler(
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
            var role = await context.Roles.FirstOrDefaultAsync(role => role.Id == request.RoleId, cancellationToken);

            if (role is null)
                return Errors.General.NotFound(null, "role").ToErrorList();

            var passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Password); 
            
            var user = User.CreateUser(
                Guid.NewGuid(),
                request.FirstName,
                request.LastName,
                request.Email,
                passwordHash,
                request.PhoneNumber,
                request.Region,
                request.Address,
                role);

            
            await context.Users.AddAsync(user, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            
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