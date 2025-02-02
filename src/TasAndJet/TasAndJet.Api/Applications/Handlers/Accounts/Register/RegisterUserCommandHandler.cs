using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common;
using TasAndJet.Api.Entities.Account;
using TasAndJet.Api.Infrastructure;

namespace TasAndJet.Api.Applications.Handlers.Accounts.Register;

public class RegisterUserCommandHandler(
    IValidator<RegisterUserCommand> validator,
    ApplicationDbContext context,
    ILogger<RegisterUserCommandHandler> logger) : IRequestHandler<RegisterUserCommand, UnitResult<ErrorList>>
{

    public async Task<UnitResult<ErrorList>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        
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

            return Result.Success<ErrorList>();
            
        }
        catch (Exception exception)
        {
            logger.LogError("Ошибка при регистрации пользователя: {@Email}, {@Phone} Ошибка: {@Message}",
                request.Email, request.PhoneNumber, exception.Message);
            await transaction.RollbackAsync(cancellationToken);

            return Error.Failure("register.user", "Ошибка при регистрации пользователя").ToErrorList();
        }
    }
}