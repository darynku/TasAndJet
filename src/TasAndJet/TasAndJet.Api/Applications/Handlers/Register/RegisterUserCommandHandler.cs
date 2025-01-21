using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TasAndJet.Api.Entities;
using TasAndJet.Api.Entities.Account;
using TasAndJet.Api.Infrastructure;
using TasAndJet.Api.Infrastructure.Repositories;

namespace TasAndJet.Api.Applications.Handlers.Register;

public class RegisterUserCommandHandler(
    IValidator<RegisterUserCommand> validator,
    ApplicationDbContext context,
    ILogger<RegisterUserCommandHandler> logger) : IRequestHandler<RegisterUserCommand, Guid>
{

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        
        
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var role = await context.Roles.FirstOrDefaultAsync(role => role.Id == request.RoleId, cancellationToken);
            
            if(role is null)
                throw new ValidationException("Role not found");

            var passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Password); 
            
            var userResult = User.CreateUser(
                Guid.NewGuid(),
                request.FirstName,
                request.LastName,
                request.Email,
                passwordHash,
                request.PhoneNumber,
                request.Region,
                request.Address,
                role);
            
            
            await context.Users.AddAsync(userResult.Value, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);

            return userResult.Value.Id;
            
        }
        catch (Exception exception)
        {
            logger.LogError("Ошибка при регистрации пользователя: {@Email}, {@Phone} Ошибка: {@Message}",
                request.Email, request.PhoneNumber, exception.Message);
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}